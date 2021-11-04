using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    public class BinanceBotDbContext : DbContext, IBinanceBotDbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        
        public BinanceBotDbContext()
        { 
            Database.Migrate();
        }

        public BinanceBotDbContext(DbContextOptions<BinanceBotDbContext> options)
            : base(options)
        { 
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=q;Persist Security Info=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("adminpack")
                .HasAnnotation("Relational:Collation", "Russian_Russia.1251");
        }
        
        public IQueryable<User> GetUsersByLogin(string login)
            => Users
                .Include(e => e.Role)
                .Where(e => e.Login == login);
    }
}