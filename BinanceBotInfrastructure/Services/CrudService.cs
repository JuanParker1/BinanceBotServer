using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;
using Mapster;

namespace BinanceBotInfrastructure.Services
{
    public class CrudService<TDto, TModel> : ICrudService<TDto>
        where TDto : BinanceBotApp.Data.IId
        where TModel : class, BinanceBotDb.Models.IId
    {
        protected readonly IBinanceBotDbContext Db;
        protected readonly DbSet<TModel> DbSet;

        public IEnumerable<string> Includes { get; } = new List<string>();

        protected CrudService(IBinanceBotDbContext db)
        {
            Db = db;
            DbSet = db.Set<TModel>();
        }

        public virtual async Task<PaginationContainer<TDto>> GetPageAsync(int skip = 0, 
            int take = 32, CancellationToken token = default)
        {
            var query = GetQueryWithIncludes();
            var count = await query
                .CountAsync(token);

            var container = new PaginationContainer<TDto>
            {
                Skip = skip,
                Take = take,
                Count = count,
            };

            if (skip >= count)
                return container;

            query = query
                .OrderBy(e => e.Id);

            if (skip > 0)
                query = query.Skip(skip);

            query = query.Take(take);

            var entities = await query
                .ToListAsync(token);

            container.Items = entities
                .Select(Convert)
                .ToList();

            return container;
        }

        public virtual async Task<TDto> GetAsync(int id, CancellationToken token)
        {
            var query = GetQueryWithIncludes();
            var entity = await query
                .FirstOrDefaultAsync(e => e.Id == id, token);
            var dto = Convert(entity);
            return dto;
        }
        
        public virtual async Task<IEnumerable<TDto>> GetAllAsync(CancellationToken token)
        {
            var entities = await (from q in GetQueryWithIncludes()
                                orderby q.Id
                                select q)
                                .ToListAsync(token);
            var dtos = entities.Select(Convert);
            return dtos;
        }

        public virtual async Task<int> InsertAsync(TDto item, CancellationToken token)
        {
            var entity = Convert(item);
            entity.Id = 0;
            DbSet.Add(entity);
            await Db.SaveChangesAsync(token);
            return entity.Id;
        }

        public virtual Task<int> InsertRangeAsync(IEnumerable<TDto> items, 
            CancellationToken token)
        {
            var entities = items.Select(i => { 
                var entity = Convert(i);
                entity.Id = 0;
                return entity;
            });

            DbSet.AddRange(entities);
            return Db.SaveChangesAsync(token);
        }

        public virtual async Task<int> UpdateAsync(int id, TDto item, 
            CancellationToken token)
        {
            var existingEntity = await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, token);
            if (existingEntity is null)
                return 0;
            var entity = Convert(item);
            entity.Id = id;
            
            DbSet.Update(entity);
            return await Db.SaveChangesAsync(token);
        }
        
        public virtual async Task<int> UpdateRangeAsync(int id, IEnumerable<TDto> items, 
            CancellationToken token)
        {
            var ids = items.Select(i => i.Id);
            var dtos = await GetExistingEntitiesAsync(ids, token);
            if (!dtos.Any())
                return 0;
            var entities = Convert(dtos);
            
            DbSet.UpdateRange(entities);

            return await Db.SaveChangesAsync(token);
        }

        public async Task<IEnumerable<TDto>> GetExistingEntitiesAsync(IEnumerable<int> ids,
            CancellationToken token)
        {
            var existingEntities = await (from entity in DbSet
                                    where ids.Contains(entity.Id)
                                    select entity)
                                    .AsNoTracking()
                                    .ToListAsync(token);
            
            return Convert(existingEntities);
        }

        public virtual Task<int> DeleteAsync(int id, CancellationToken token)
        {
            var entity = DbSet.AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
            if (entity == default)
                return Task.FromResult(0);
            
            DbSet.Remove(entity);
            return Db.SaveChangesAsync(token);
        }

        public virtual Task<int> DeleteRangeAsync(IEnumerable<int> ids, 
            CancellationToken token)
        {
            var entities = (from entity in DbSet
                            where ids.Contains(entity.Id)
                            select entity)
                            .AsNoTracking();
            
            if (entities == default)
                return Task.FromResult(0);
            
            DbSet.RemoveRange(entities);
            return Db.SaveChangesAsync(token);
        }
        
        protected virtual TDto Convert(TModel src) => src.Adapt<TDto>();
        
        protected virtual IEnumerable<TDto> Convert(IEnumerable<TModel> src) => 
            src.Adapt<IEnumerable<TDto>>();

        protected virtual TModel Convert(TDto src) => src.Adapt<TModel>();
        protected virtual IEnumerable<TModel> Convert(IEnumerable<TDto> src) => 
            src.Adapt<IEnumerable<TModel>>();

        private IQueryable<TModel> GetQueryWithIncludes() =>
            Includes.Aggregate<string, IQueryable<TModel>>(DbSet, 
                (current, include) => current.Include(include));
    }
}