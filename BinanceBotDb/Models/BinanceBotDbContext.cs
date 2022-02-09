using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    public class BinanceBotDbContext : DbContext, IBinanceBotDbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Settings> UserSettings { get; set; }

        public BinanceBotDbContext()
        {
            //Database.Migrate();
        }

        public BinanceBotDbContext(DbContextOptions<BinanceBotDbContext> options)
            : base(options)
        {
            //Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Database=binanceBotDb;Username=postgres;Password=q;Persist Security Info=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => { entity.HasIndex(d => d.Login).IsUnique(); });
            
            FillData(modelBuilder);
        }
        
        private static void FillData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasData(new List<UserRole>
                {
                    new UserRole {Id = 1, Caption = "Administrator",},
                    new UserRole {Id = 2, Caption = "User",},
                });
            });
        }

        public IQueryable<User> GetUserByLogin(string login)
            => Users.Include(e => e.Role)
                .Where(e => e.Login == login);
    }
}