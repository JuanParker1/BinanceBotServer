using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services.Cache
{
    public class CacheDb
    {
        private readonly ConcurrentDictionary<string, CacheTableDataStorage> _cache = 
            new ConcurrentDictionary<string, CacheTableDataStorage>();

        public CacheTable<TEntity> GetCachedTable<TEntity>(DbContext context, ISet<string> includes = null)
            where TEntity : class
        {
            var cacheItem = GetCacheTableDataStore<TEntity>();
            var tableCache = new CacheTable<TEntity>(context, cacheItem, includes);
            return tableCache;
        }

        private CacheTableDataStorage GetCacheTableDataStore<TEntity>()
            where TEntity : class
        {
            var nameOfTEntity = typeof(TEntity).FullName;
            var cacheItem = _cache.GetOrAdd(nameOfTEntity, (nameOfTEntity) => new CacheTableDataStorage
            {
                NameOfTEntity = nameOfTEntity,
                Entities = new List<TEntity>(),
            });
        
            return cacheItem;
        }

        public void DropAll() => _cache.Clear();

        public void Drop<TEntity>() => _cache.Remove(typeof(TEntity).FullName, out _);
    }
}