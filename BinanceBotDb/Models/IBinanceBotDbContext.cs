using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotDb.Models.Directories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BinanceBotDb.Models
{
    public interface IBinanceBotDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<Settings> UserSettings { get; set; }
        DbSet<BalanceChange> BalanceChanges { get; set; }
        DbSet<Request> RequestLog { get; set; }
        DbSet<Event> EventLog { get; set; }
        DbSet<EventTemplate> EventTemplates { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderType> OrderTypes { get; set; }
        DbSet<TradeMode> TradeModes { get; set; }
        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        IQueryable<User> GetUserByLogin(string login);
    }
}