using System.Collections.Generic;
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
                optionsBuilder.UseNpgsql(
                    "Host=localhost;Database=postgres;Username=postgres;Password=q;Persist Security Info=True");
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
            
             modelBuilder.Entity<User>(entity =>
             {
                 entity.HasData(new List<User> {
                     new User
                     {
                         Id = 1,
                         IdRole = 1,
                         Login = "dev",
                         PasswordHash =
                             "Vlcjq4fa529103dde7ff72cfe76185f344d4aa87931f8e1b2044e8a7739947c3d18923464eaad93843e4f809c5e126d013072",
                         Name = "Developer",
                     },
                 });
             });
        }

        public IQueryable<User> GetUsersByLogin(string login)
            => Users.Include(e => e.Role)
                .Where(e => e.Login == login);
    }
}