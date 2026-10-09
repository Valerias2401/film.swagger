using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prilozhenie.DatabaseContext;
using prilozhenie.Models;

namespace prilozhenie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ContextDb _context;
        public ChatController(ContextDb context) => _context = context;

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetMovieMessages(int movieId)
        {
            var messages = await _context.ChatMessages
                .Include(m => m.FromUser)
                .Where(m => m.MovieId == movieId && m.ToUserId == null && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    MovieId = m.MovieId,
                    FromUserId = m.FromUserId,
                    FromUserName = m.FromUser!.Name,
                    ToUserId = m.ToUserId,
                    Content = m.Content,
                    PhotoUrl = m.PhotoUrl,
                    CreatedAt = m.CreatedAt,
                    IsEdited = m.IsEdited
                })
                .ToListAsync();

            return Ok(new { status = true, messages });
        }

        [HttpGet("private/{withUserId}")]
        public async Task<IActionResult> GetPrivateMessages(int withUserId)
        {
            int? currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return Unauthorized(new { status = false, message = "Не авторизован" });

            var messages = await _context.ChatMessages
                .Include(m => m.FromUser)
                .Where(m =>
                    !m.IsDeleted &&
                    ((m.FromUserId == currentUserId.Value && m.ToUserId == withUserId) ||
                     (m.FromUserId == withUserId && m.ToUserId == currentUserId.Value)))
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    FromUserId = m.FromUserId,
                    FromUserName = m.FromUser!.Name,
                    ToUserId = m.ToUserId,
                    Content = m.Content,
                    PhotoUrl = m.PhotoUrl,
                    CreatedAt = m.CreatedAt,
                    IsEdited = m.IsEdited
                })
                .ToListAsync();

            return Ok(new { status = true, messages });
        }

        private int? GetCurrentUserId()
        {
            var header = Request.Headers["X-User-Id"].FirstOrDefault();
            return int.TryParse(header, out int id) ? id : null;
        }
    }
}