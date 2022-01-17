using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace BinanceBotDb.Models
{
    public interface IBinanceBotDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<TEntity> Set<TEntity>(string name) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        IQueryable<User> GetUserByLogin(string login);
    }
}