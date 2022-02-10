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
        protected readonly IBinanceBotDbContext Context;
        protected readonly DbSet<TModel> DbSet;

        public IEnumerable<string> Includes { get; } = new List<string>();

        public CrudService(IBinanceBotDbContext context)
        {
            this.Context = context;
            DbSet = context.Set<TModel>();
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
                .Select(entity => Convert(entity))
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
            var query = GetQueryWithIncludes();
            var entities = await query
                .OrderBy(e => e.Id)
                .ToListAsync(token);
            var dto = entities.Select(Convert).ToList();
            return dto;
        }

        public virtual async Task<int> InsertAsync(TDto item, CancellationToken token)
        {
            var entity = Convert(item);
            entity.Id = 0;
            DbSet.Add(entity);
            await Context.SaveChangesAsync(token);
            return entity.Id;
        }

        public virtual Task<int> InsertRangeAsync(IEnumerable<TDto> items, CancellationToken token)
        {
            var entities = items.Select(i => { 
                var entity = Convert(i);
                entity.Id = 0;
                return entity;
            });

            DbSet.AddRange(entities);
            return Context.SaveChangesAsync(token);
        }

        public virtual async Task<int> UpdateAsync(int id, TDto item, CancellationToken token)
        {
            var existingEntity = await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, token);
            if (existingEntity is null)
                return 0;
            var entity = Convert(item);
            entity.Id = id;
            DbSet.Update(entity);
            return await Context.SaveChangesAsync(token);
        }

        public virtual Task<int> DeleteAsync(int id, CancellationToken token)
        {
            var entity = DbSet.AsNoTracking()
                .FirstOrDefault(e => e.Id == id);
            if (entity == default)
                return Task.FromResult(0);
            
            DbSet.Remove(entity);
            return Context.SaveChangesAsync(token);
        }

        public virtual Task<int> DeleteRangeAsync(IEnumerable<int> ids, CancellationToken token)
        {
            var entities = DbSet.Where(e => ids.Contains(e.Id))
                .AsNoTracking();
            if (entities == default)
                return Task.FromResult(0);
            
            DbSet.RemoveRange(entities);
            return Context.SaveChangesAsync(token);
        }
        
        protected virtual TDto Convert(TModel src) => src.Adapt<TDto>();

        protected virtual TModel Convert(TDto src) => src.Adapt<TModel>();

        private IQueryable<TModel> GetQueryWithIncludes() =>
            Includes.Aggregate<string, IQueryable<TModel>>(DbSet, 
                (current, include) => current.Include(include));
    }
}