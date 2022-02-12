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
        private static readonly SemaphoreSlim _semaphore = new(2); // TODO: Что-то сразу съедает 1 из них.
        private static readonly TimeSpan _minPeriodRefresh = TimeSpan.FromSeconds(5);
        private static readonly string _nameOfTEntity = typeof(TEntity).Name;

        private readonly CacheTableDataStorage _data;
        private readonly Func<DbSet<TEntity>, IQueryable<TEntity>> _configureDbSet;
        private readonly List<TEntity> _cached;
        private readonly DbContext _db;
        private readonly DbSet<TEntity> _dbSet;

        internal CacheTable(DbContext db, CacheTableDataStorage data, ISet<string> includes = null)
        {
            _db = db;
            _data = data;
            _dbSet = db.Set<TEntity>();
            
            if (includes is not null && includes.Any())
                _configureDbSet = (dbSet) =>
                    includes.Aggregate<string, IQueryable<TEntity>>(dbSet, 
                        (current, include) => current.Include(include));
                

            _cached = (List<TEntity>)data.Entities;
            if ((_cached.Count == 0) || data.IsObsolete)
                Refresh(false);
        }

        internal CacheTable(DbContext db, CacheTableDataStorage data,
            Func<DbSet<TEntity>, IQueryable<TEntity>> configureDbSet = null)
        {
            _db = db;
            _data = data;
            _configureDbSet = configureDbSet;

            _dbSet = db.Set<TEntity>();

            _cached = (List<TEntity>)data.Entities;
            if ((_cached.Count == 0) || data.IsObsolete)
                Refresh(false);
        }

        public TEntity this[int index] => 
            _cached.ElementAt(index);

        /// <summary>
        /// Runs action like atomic operation.
        /// hasFree is action argument indicates that semaphore was threads
        /// to enter SemaphoreSlim object.
        /// It may be needed to avoid multiple operations like Refresh().
        /// </summary>
        /// <param name="func"> (hasFree) => {...} </param>
        /// <returns>default if semaphoreTimeout. Or result of func(..)</returns>
        private static T Sync<T>(Func<bool, T> func)
        {
            if (func is null || !_semaphore.Wait(_semaphoreTimeout))
                return default;
   
            var hasFree = _semaphore.CurrentCount > 0;
            T result = default;
            
            try
            {
                result = func.Invoke(hasFree);
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
        /// Runs action like atomic operation.
        /// hasFree is action argument indicates that semaphore was threads
        /// to enter SemaphoreSlim object.
        /// It may be needed to avoid multiple operations like Refresh().
        /// </summary>
        /// <param name="funcAsync"> (hasFree) => {...} </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> default if semaphoreTimeout. Or result of func(..) </returns>
        private static async Task<T> SyncAsync<T>(Func<bool, CancellationToken, Task<T>> funcAsync,
            CancellationToken token = default)
        {
            if (funcAsync is null || !await _semaphore.WaitAsync(_semaphoreTimeout, token))
                return default;
            
            var hasFree = _semaphore.CurrentCount > 0;
            T result = default;

            try
            {
                result = await funcAsync.Invoke(hasFree, token);
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
                : _configureDbSet(_dbSet);
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
                : _configureDbSet(_dbSet);
            var queryEntities = await query.AsNoTracking()
                .ToListAsync(token);
            _cached.AddRange(queryEntities);
            _data.LastRefreshDate = DateTime.Now;

            return _cached.Count;
        }

        public int Refresh(bool force)
            => Sync((hasFree) => hasFree ? InternalRefresh(force) : 0);

        public Task<int> RefreshAsync(bool force, CancellationToken token = default) =>
            SyncAsync(
                async (hasFree, token) => hasFree ? await InternalRefreshAsync(force, token) : 0, token);

        public bool Contains(Func<TEntity, bool> predicate)
            => FirstOrDefault(predicate) != default;

        public async Task<bool> ContainsAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default) => 
            await FirstOrDefaultAsync(predicate, token) != default;

        public TEntity GetOrCreate(Func<TEntity, bool> predicate, Func<TEntity> makeNew)
            => Sync(hasFree =>
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

            Refresh(false);
            return _cached.FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync(CancellationToken token = default)
        {
            var result = _cached.FirstOrDefault();
            if (result != default)
                return result;

            await RefreshAsync(false, token);
            return _cached.FirstOrDefault();
        }

        public TEntity FirstOrDefault(Func<TEntity, bool> predicate)
        {
            var result = _cached.FirstOrDefault(predicate);
            if (result != default)
                return result;

            Refresh(false);
            return _cached.FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default)
        {
            var result = _cached.FirstOrDefault(predicate);
            if (result != default)
                return result;

            await RefreshAsync(false, token);
            return _cached.FirstOrDefault(predicate);
        }
        
        public TEntity LastOrDefault(Func<TEntity, bool> predicate)
        {
            var result = _cached.LastOrDefault(predicate);
            if (result != default)
                return result;

            Refresh(false);
            return _cached.LastOrDefault(predicate);
        }
        
        public async Task<TEntity> LastOrDefaultAsync(Func<TEntity, bool> predicate, 
            CancellationToken token = default)
        {
            var result = _cached.LastOrDefault(predicate);
            if (result != default)
                return result;

            await RefreshAsync(false, token);
            return _cached.LastOrDefault(predicate);
        }

        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate = default)
        {
            var result = predicate != default
                ? _cached.Where(predicate)
                : _cached;
            
            if (result.Any())
                return result;

            Refresh(false);
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

            await RefreshAsync(false, token);
            result = (predicate != default)
                ? _cached.Where(predicate)
                : _cached;
            return result;
        }

        public int Upsert(TEntity entity)
        {
            if (entity == default)
                return 0;
            
            return Sync((hasFree) =>
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
            => SyncAsync(async (hasFree, token) =>
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

            return Sync((hasFree) =>
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

            return SyncAsync(async (hasFree, token) =>
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
            => Sync(_ =>
            {
                _dbSet.RemoveRange(_dbSet.Where(predicate));
                var affected = _db.SaveChanges();
                if (affected > 0)
                    InternalRefresh(true);
                
                return affected;
            });

        public Task<int> RemoveAsync(Func<TEntity, bool> predicate, CancellationToken token = default)
            => SyncAsync(async (hasFree, token) =>
            {
                _dbSet.RemoveRange(_dbSet.Where(predicate));
                var affected = await _db.SaveChangesAsync(token);
                if (affected > 0)
                    await InternalRefreshAsync(true, token);
                
                return affected;
            }, token);

        public TEntity Insert(TEntity entity)
        {
            return Sync(_ =>
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
            return SyncAsync(async (hasFree, token) =>
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

            return Sync(_ =>
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

            return SyncAsync(async (hasFree, token) =>
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