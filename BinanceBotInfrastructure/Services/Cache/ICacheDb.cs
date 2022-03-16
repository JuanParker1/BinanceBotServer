using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services.Cache
{
    public interface ICacheDb
    {
        CacheTable<TEntity> GetCachedTable<TEntity>(DbContext context, ISet<string> includes = null)
            where TEntity : class;
        void DropAll();
        void Drop<TEntity>();
    }
}