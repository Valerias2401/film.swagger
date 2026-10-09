using Microsoft.EntityFrameworkCore;
using prilozhenie.Models;

namespace prilozhenie.DatabaseContext
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}