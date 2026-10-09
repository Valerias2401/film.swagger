using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using prilozhenie.DatabaseContext;
using prilozhenie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prilozhenie.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ContextDb _context;
        private static readonly Dictionary<int, string> _connectedUsers = new Dictionary<int, string>();
        private static readonly HashSet<string> _processingMessages = new HashSet<string>();

        public ChatHub(ContextDb context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            int? userId = GetUserIdFromQuery();

            if (userId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id_User == userId.Value);

                if (userExists)
                {
                    if (_connectedUsers.ContainsKey(userId.Value))
                    {
                        _connectedUsers.Remove(userId.Value);
                    }

                    _connectedUsers[userId.Value] = Context.ConnectionId;

                    await Clients.All.SendAsync("UserOnline", userId.Value);

                    var onlineUserIds = _connectedUsers.Keys.ToList();
                    await Clients.Caller.SendAsync("ReceiveOnlineUsers", onlineUserIds);
                }
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            int? userId = GetUserIdFromQuery();
            if (userId.HasValue && _connectedUsers.ContainsKey(userId.Value))
            {
                _connectedUsers.Remove(userId.Value);

                await Clients.All.SendAsync("UserOffline", userId.Value);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinMovieChat(int movieId)
        {
            int? userId = GetUserIdFromQuery();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"movie_{movieId}");
            }
        }

        public async Task JoinPrivateChat(int withUserId)
        {
            int? currentUserId = GetUserIdFromQuery();
            if (currentUserId.HasValue)
            {
                string groupName = GetPrivateGroupName(currentUserId.Value, withUserId);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }
        }

        public async Task SendMessage(int? movieId, int? toUserId, string content, string? photoUrl = null)
        {
            int? fromUserId = GetUserIdFromQuery();
            if (!fromUserId.HasValue) throw new HubException("Не авторизован");

            var messageKey = $"{fromUserId}_{toUserId}_{content}_{DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond}";

            if (_processingMessages.Contains(messageKey))
            {
                Console.WriteLine($"Duplicate message detected: {messageKey}");
                return;
            }

            _processingMessages.Add(messageKey);

            _ = Task.Delay(2000).ContinueWith(_ => _processingMessages.Remove(messageKey));

            var message = new ChatMessage
            {
                MovieId = movieId,
                FromUserId = fromUserId.Value,
                ToUserId = toUserId,
                Content = content ?? "",
                PhotoUrl = photoUrl,
                CreatedAt = DateTime.UtcNow,
                IsEdited = false,
                IsDeleted = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            var dto = new
            {
                messageId = message.MessageId,
                movieId = message.MovieId,
                fromUserId = message.FromUserId,
                fromUserName = await GetUserNameAsync(message.FromUserId),
                toUserId = message.ToUserId,
                toUserName = message.ToUserId.HasValue ? await GetUserNameAsync(message.ToUserId.Value) : null,
                content = message.Content,
                photoUrl = message.PhotoUrl,
                createdAt = message.CreatedAt,
                isEdited = message.IsEdited,
                isDeleted = message.IsDeleted
            };

            if (toUserId.HasValue)
            {
                string groupName = GetPrivateGroupName(fromUserId.Value, toUserId.Value);
                await Clients.Group(groupName).SendAsync("ReceiveMessage", dto);
            }
            else if (movieId.HasValue)
            {
                await Clients.Group($"movie_{movieId}").SendAsync("ReceiveMessage", dto);
            }
        }

        public async Task EditMessage(int messageId, string newContent)
        {
            int? userId = GetUserIdFromQuery();
            if (!userId.HasValue) return;

            var msg = await _context.ChatMessages.FindAsync(messageId);
            var user = await _context.Users.FindAsync(userId.Value);

            if (msg != null && (msg.FromUserId == userId.Value || user?.RoleId == 1))
            {
                msg.Content = newContent;
                msg.IsEdited = true;
                await _context.SaveChangesAsync();

                var dto = new { messageId = msg.MessageId, content = msg.Content, isEdited = true };

                if (msg.ToUserId.HasValue)
                {
                    string group = GetPrivateGroupName(msg.FromUserId, msg.ToUserId.Value);
                    await Clients.Group(group).SendAsync("MessageEdited", dto);
                }
                else if (msg.MovieId.HasValue)
                {
                    await Clients.Group($"movie_{msg.MovieId}").SendAsync("MessageEdited", dto);
                }
            }
        }

        public async Task DeleteMessage(int messageId)
        {
            int? userId = GetUserIdFromQuery();
            if (!userId.HasValue) return;

            var msg = await _context.ChatMessages.FindAsync(messageId);
            var user = await _context.Users.FindAsync(userId.Value);

            if (msg != null && (msg.FromUserId == userId.Value || user?.RoleId == 1))
            {
                msg.IsDeleted = true;
                await _context.SaveChangesAsync();

                if (msg.ToUserId.HasValue)
                {
                    string group = GetPrivateGroupName(msg.FromUserId, msg.ToUserId.Value);
                    await Clients.Group(group).SendAsync("MessageDeleted", messageId);
                }
                else if (msg.MovieId.HasValue)
                {
                    await Clients.Group($"movie_{msg.MovieId}").SendAsync("MessageDeleted", messageId);
                }
            }
        }

        public async Task<List<int>> GetOnlineUsers()
        {
            return _connectedUsers.Keys.ToList();
        }

        private static string GetPrivateGroupName(int u1, int u2) => $"private_{Math.Min(u1, u2)}_{Math.Max(u1, u2)}";

        private int? GetUserIdFromQuery()
        {
            var query = Context.GetHttpContext()?.Request.Query;
            return int.TryParse(query?["userId"], out int id) ? id : null;
        }

        private async Task<string> GetUserNameAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user?.Name ?? "Unknown";
        }
    }
}