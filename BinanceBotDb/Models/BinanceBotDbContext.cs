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
                 entity.HasData(new List<User> 
                 {
                     new User
                     {
                         Id = 1,
                         IdRole = 1,
                         Login = "dev",
                         Password =
                             "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91",
                         Name = "Developer",
                     },
                 });
             });

             modelBuilder.Entity<Settings>(entity =>
             {
                 entity.HasData(new List<Settings>
                 {
                     new Settings
                     {
                         Id = 1,
                         IdUser = 1,
                         IsTradeEnabled = false,
                         TradeMode = 0,
                         LimitOrderRate = 25
                    }
                 });
             });
        }

        public IQueryable<User> GetUserByLogin(string login)
            => Users.Include(e => e.Role)
                .Where(e => e.Login == login);
    }
}