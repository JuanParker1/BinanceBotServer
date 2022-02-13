using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace BinanceBotInfrastructure.Services.Cache
{
    public class CacheTable<TEntity>
        where TEntity : class
    {
        private const int _semaphoreTimeout = 5000;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static readonly TimeSpan _minPeriodRefresh = TimeSpan.FromSeconds(5);
        private static readonly string _nameOfTEntity = typeof(TEntity).Name;

        private readonly CacheTableDataStorage _data;
        private readonly Func<DbSet<TEntity>, IQueryable<TEntity>> _configureDbSet;
        private readonly List<TEntity> _cached;
        private readonly DbContext _db;
        private readonly DbSet<TEntity> _dbSet;

        internal CacheTable(DbContext db, CacheTableDataStorage data, 
            ISet<string> includes = null)
        {
            _db = db;
            _data = data;
            _dbSet = db.Set<TEntity>();
            
            if (includes is not null && includes.Any())
                _configureDbSet = (dbSet) =>
                    includes.Aggregate<string, IQueryable<TEntity>>(dbSet, 
                        (current, include) => 
                            current.Include(include));

            _cached = (List<TEntity>)data.Entities;
            if (_cached.Count == 0 || data.IsObsolete)
                Sync(() => InternalRefresh(true));
        }

        public TEntity this[int index] => 
            _cached.ElementAt(index);

        /// <summary>
        /// Runs delegate as atomic operation. Delegates are synchronized by
        /// SemaphoreSlim object.
        /// It may be needed to avoid multiple operations like Refresh().
        /// </summary>
        /// <param name="func"> Semaphore synchronized function </param>
        /// <returns> Result of func(..) </returns>
        private static T Sync<T>(Func<T> func)
        {
            T result = default;
            
            try
            {
                if (func is null || !_semaphore.Wait(_semaphoreTimeout))
                    throw new Exception("Sync function was null or " +
                                        "semaphore wait timeout was exceeded.");
                result = func.Invoke();
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss:fff} error in " +
                                $"CacheTable<{_nameOfTEntity}>.Sync()");
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace); // TODO: Разобраться, где тут TraceListener и такое же сделать в Program.cs, где .Migrate() может падать.
            }
            finally
            {
                _semaphore.Release();
            }

            return result;
        }

        /// <summary>
        /// Runs delegate as atomic operation. Delegates are synchronized by
        /// SemaphoreSlim object.
        /// </summary>
        /// <param name="funcAsync"> Semaphore synchronized function </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Result of func(..) </returns>
        private static async Task<T> SyncAsync<T>(Func<CancellationToken, Task<T>> funcAsync,
            CancellationToken token = default)
        {
            T result = default;

            try
            {
                if (funcAsync is null || !await _semaphore.WaitAsync(_semaphoreTimeout, token))
                    throw new Exception("Sync function was null or " +
                                        "semaphore wait timeout was exceeded.");
                result = await funcAsync.Invoke(token);
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"{DateTime.Now:yyyy.MM.dd HH:mm:ss:fff} error in " +
                                $"CacheTable<{_nameOfTEntity}>.SyncAsync()");
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
            finally
            {
                _semaphore.Release();
            }

            return result;
        }

        private int InternalRefresh(bool force)
        {
            if (!force && _data.LastRefreshDate + _minPeriodRefresh >= DateTime.Now) 
                return _cached.Count;
            
            _cached.Clear();
            var queryEntities = _configureDbSet is null 
                ? _dbSet 
                : _configureDbSet.Invoke(_dbSet);

            var entities = queryEntities.AsNoTracking().ToList();
            _cached.AddRange(entities);
            _data.LastRefreshDate = DateTime.Now;

            return _cached.Count;
        }

        private async Task<int> InternalRefreshAsync(bool force, CancellationToken token = default)
        {
            if (!force && _data.LastRefreshDate + _minPeriodRefresh >= DateTime.Now) 
                return _cached.Count;
            
            _cached.Clear();
            var query = _configureDbSet is null 
                ? _dbSet 
                : _configureDbSet.Invoke(_dbSet);
            var queryEntities = await query.AsNoTracking()
                .ToListAsync(token);
            _cached.AddRange(queryEntities);
            _data.LastRefreshDate = DateTime.Now;

            return _cached.Count;
        }

        public bool Contains(Func<TEntity, bool> predicate)
            => FirstOrDefault(predicate) != default;

        public async Task<bool> ContainsAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default) => 
            await FirstOrDefaultAsync(predicate, token) != default;

        public TEntity GetOrCreate(Func<TEntity, bool> predicate, Func<TEntity> makeNew)
            => Sync(() =>
            {
                var result = _cached.FirstOrDefault(predicate);
                if (result != default)
                    return result;

                InternalRefresh(true);
                result = _cached.FirstOrDefault(predicate);
                if (result != default)
                    return result;

                var entry = _dbSet.Add(makeNew());
                _db.SaveChanges();
                InternalRefresh(true);
                return entry.Entity;
            });

        public TEntity FirstOrDefault()
        {
            var result = _cached.FirstOrDefault();
            if (result != default)
                return result;

            Sync(() => InternalRefresh(false));
            return _cached.FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync(CancellationToken token = default)
        {
            var result = _cached.FirstOrDefault();
            if (result != default)
                return result;

            await SyncAsync(
                async (token) => await InternalRefreshAsync(false, token), token);
            
            return _cached.FirstOrDefault();
        }

        public TEntity FirstOrDefault(Func<TEntity, bool> predicate)
        {
            var result = _cached.FirstOrDefault(predicate);
            if (result != default)
                return result;

            Sync(() => InternalRefresh(false));
            return _cached.FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default)
        {
            var result = _cached.FirstOrDefault(predicate);
            if (result != default)
                return result;

            await SyncAsync(
                async (token) => await InternalRefreshAsync(false, token), token);
            
            return _cached.FirstOrDefault(predicate);
        }
        
        public TEntity LastOrDefault(Func<TEntity, bool> predicate)
        {
            var result = _cached.LastOrDefault(predicate);
            if (result != default)
                return result;

            Sync(() => InternalRefresh(false));
            return _cached.LastOrDefault(predicate);
        }
        
        public async Task<TEntity> LastOrDefaultAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default)
        {
            var result = _cached.LastOrDefault(predicate);
            if (result != default)
                return result;

            await SyncAsync(
                async (token) => await InternalRefreshAsync(false, token), token);
            
            return _cached.LastOrDefault(predicate);
        }

        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate = default)
        {
            var result = predicate != default
                ? _cached.Where(predicate)
                : _cached;
            
            if (result.Any())
                return result;

            Sync(() => InternalRefresh(false));
            result = (predicate != default)
                ? _cached.Where(predicate)
                : _cached;
            return result;
        }

        public Task<IEnumerable<TEntity>> WhereAsync(CancellationToken token = default) =>
            WhereAsync(default, token);

        public async Task<IEnumerable<TEntity>> WhereAsync(Func<TEntity, bool> predicate = default,
            CancellationToken token = default)
        {
            var result = (predicate != default)
                ? _cached.Where(predicate)
                : _cached;
            if (result.Any())
                return result;

            await SyncAsync(
                async (token) => await InternalRefreshAsync(false, token), token);
            
            result = (predicate != default)
                ? _cached.Where(predicate)
                : _cached;
            return result;
        }

        public int Upsert(TEntity entity)
        {
            if (entity == default)
                return 0;
            
            return Sync(() =>
            {
                if (_dbSet.Contains(entity))
                    _dbSet.Update(entity);
                else
                    _dbSet.Add(entity);
                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                return affected;
            });
        }

        public Task<int> UpsertAsync(TEntity entity, CancellationToken token = default)
            => SyncAsync(async (token) =>
            {
                if (_dbSet.Contains(entity))
                    _dbSet.Update(entity);
                else
                    _dbSet.Add(entity);
                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                return affected;
            }, token);

        public int Upsert(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                return 0;

            return Sync(() =>
            {
                foreach (var entity in entities)
                {
                    if (_dbSet.Contains(entity))
                        _dbSet.Update(entity);
                    else
                        _dbSet.Add(entity);
                }

                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                
                return affected;
            });
        }

        public Task<int> UpsertAsync(IEnumerable<TEntity> entities, CancellationToken token = default)
        {
            if (!entities.Any())
                return Task.FromResult(0);

            return SyncAsync(async (token) =>
            {
                var upsertedEntries = new List<TEntity>(entities.Count());
                foreach (var entity in entities)
                {
                    if (_dbSet.Contains(entity))
                        _dbSet.Update(entity);
                    else
                        _dbSet.Add(entity);
                }

                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                return affected;
            }, token);
        }

        public int Remove(Func<TEntity, bool> predicate)
            => Sync(() =>
            {
                _dbSet.RemoveRange(_dbSet.Where(predicate));
                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                
                return affected;
            });

        public Task<int> RemoveAsync(Func<TEntity, bool> predicate, CancellationToken token = default)
            => SyncAsync(async (token) =>
            {
                _dbSet.RemoveRange(_dbSet.Where(predicate));
                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                
                return affected;
            }, token);

        public TEntity Insert(TEntity entity)
        {
            return Sync(() =>
            {
                var entry = _dbSet.Add(entity);
                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                
                return entry.Entity;
            });
        }

        public Task<TEntity> InsertAsync(TEntity entity, CancellationToken token = default)
        {
            return SyncAsync(async (token) =>
            {
                var entry = _dbSet.Add(entity);
                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                
                return entry.Entity;
            }, token);
        }

        public IEnumerable<TEntity> Insert(IEnumerable<TEntity> newEntities)
        {
            if (newEntities is null)
                return null;
            var count = newEntities.Count();
            if (count == 0)
                return null;

            return Sync(() =>
            {
                var entries = new List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>>(count);
                entries.AddRange(newEntities.Select(newEntity => _dbSet.Add(newEntity)));

                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                else
                    return null;

                return entries.Select(e => e.Entity);
            });
        }

        public Task<IEnumerable<TEntity>> InsertAsync(IEnumerable<TEntity> newEntities, CancellationToken token = default)
        {
            if(newEntities is null)
                return null;
            
            var count = newEntities.Count();
            if (count == 0)
                return null;

            return SyncAsync(async (token) =>
            {
                var entries = new List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>>(count);
                entries.AddRange(newEntities.Select(newEntity => _dbSet.Add(newEntity)));
                
                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                else
                    return null;

                return entries.Select(e => e.Entity);
            }, token);
        }
    }
}