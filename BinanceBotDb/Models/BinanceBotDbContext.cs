using System.Collections.Generic;
using System.Linq;
using BinanceBotDb.Models.Directories;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    public class BinanceBotDbContext : DbContext, IBinanceBotDbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Settings> UserSettings { get; set; }
        public virtual DbSet<BalanceChange> BalanceChanges { get; set; }
        public virtual DbSet<Request> RequestLog { get; set; }
        public virtual DbSet<Event> EventLog { get; set; }
        public virtual DbSet<EventTemplate> EventTemplates { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<TradeMode> TradeModes { get; set; }

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
            
            modelBuilder.Entity<TradeMode>(entity =>
            {
                entity.HasData(new List<TradeMode>
                {
                    new TradeMode {Id = 1, Caption = "auto",},
                    new TradeMode {Id = 2, Caption = "semiAuto",},
                });
            });
            
            modelBuilder.Entity<EventTemplate>(entity =>
            {
                entity.HasData(new List<EventTemplate>
                {
                    new EventTemplate
                    {
                        Id = 1, 
                        Template = "Совершена покупка торговой пары {} в количестве {} шт. по курсу {} USDT " +
                                   "на сумму {} USDT. Дата: {} Время: {}."
                    },
                    new EventTemplate
                    {
                        Id = 2, 
                        Template = "Совершена продажа торговой пары {} в количестве {} шт. по курсу {} USDT " +
                                   "на сумму {} USDT. Дата: {} Время: {}.",
                    },
                    new EventTemplate
                    {
                        Id = 3, 
                        Template = "На бирже установлен лимитный ордер для торговой пары {} в количестве {} шт. " +
                                   "по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}."
                    },
                    new EventTemplate
                    {
                        Id = 4, 
                        Template = "На бирже отменен лимитный ордер для торговой пары {} в количестве {} шт. " +
                                   "по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.",
                    },
                    new EventTemplate
                    {
                        Id = 5, 
                        Template = "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. " +
                                   "по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\n" +
                                   "Текст ошибки: {}.",
                    },
                    new EventTemplate
                    {
                        Id = 6, 
                        Template = "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. " +
                                   "по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\n" +
                                   "Текст ошибки: {}.",
                    }
                });
            });
            
            modelBuilder.Entity<OrderType>(entity =>
            {
                entity.HasData(new List<OrderType>
                {
                    new OrderType {Id = 1, Caption = "LIMIT"},
                    new OrderType {Id = 2, Caption = "MARKET"},
                    new OrderType {Id = 3, Caption = "STOP_LOSS"},
                });
            });
        }

        public IQueryable<User> GetUserByLogin(string login)
            => Users.Include(e => e.Role)
                .Where(e => e.Login == login);
    }
}