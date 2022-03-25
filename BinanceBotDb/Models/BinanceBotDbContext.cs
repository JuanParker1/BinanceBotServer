using System.Collections.Generic;
using System.Linq;
using BinanceBotDb.Models.Directories;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotDb.Models
{
    // TODO: Move all EF Core queries from services to IRepository class (Data Access Layer)
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
            
            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.HasData(new List<OrderStatus>
                {
                    new OrderStatus {Id = 1, Caption = "NEW"},
                    new OrderStatus {Id = 2, Caption = "CANCELLED"},
                    new OrderStatus {Id = 3, Caption = "FILLED"}
                });
            });
            
            modelBuilder.Entity<EventTemplate>(entity =>
            {
                entity.HasData(new List<EventTemplate>
                {
                    new EventTemplate
                    {
                        Id = 1, 
                        Template = "Создан ордер на {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT " +
                                   "на сумму {4} USDT. \n Способ: {5}. \n  Дата: {6}"
                    },
                    new EventTemplate
                    {
                        Id = 2, 
                        Template = "Произошла ошибка при создании ордера на {0} торговой пары {1} в количестве {2} шт. " +
                                   "по курсу {3} USDT на сумму {4} USDT. \n Текст ошибки: {5} \n " +
                                   "Способ: {6}. \n Дата: {7}"
                    },
                    new EventTemplate
                    {
                        Id = 3, 
                        Template = "Совершена {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT " +
                                   "на сумму {4} USDT. \n Ордер выполнен. \n Дата: {5}"
                    },
                    new EventTemplate
                    {
                        Id = 4, 
                        Template = "Произошла отмена ордера на {0} торговой пары {1} в количестве {2} шт. " +
                                   "по курсу {3} USDT на сумму {4} USDT. \n Способ: {5} Дата: {6}"
                    },
                    new EventTemplate
                    {
                        Id = 5, 
                        Template = "Произошла ошибка при отмене ордера на {0} торговой пары {1} в количестве {2} шт. " +
                                   "по курсу {3} USDT на сумму {4} USDT. \n Текст ошибки: {5} \n " +
                                   "Способ: {6}. \n Дата: {7}"
                    },
                    new EventTemplate
                    {
                        Id = 6, 
                        Template = "Произошла ошибка при чтении ответа от биржи при {0} торговой пары {1} " +
                                   "в количестве {2} шт. по курсу {3} USDT. \n Текст ошибки: {4} \n " +
                                   "Способ: {5}. \n Дата: {6}"
                    },
                    new EventTemplate
                    {
                        Id = 7, 
                        Template = "Автоматическая торговля {0}. \n Дата: {1}"
                    },
                    new EventTemplate
                    {
                        Id = 8, 
                        Template = "Запрошена продажа всей криптовалюты на аккаунте. \n Дата: {0}"
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