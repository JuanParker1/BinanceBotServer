using System;
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
    public class EventService : CrudService<EventDto, Event>, IEventService
    {
        public EventService(IBinanceBotDbContext db) : base(db) { }
        
        public async Task<IEnumerable<EventDto>> GetAllAsync(int idUser, 
            bool isUnreadRequested, DateTime intervalStart, DateTime intervalEnd, 
            CancellationToken token)
        {
            var start = DateTime.MinValue;
            var end = DateTime.Now;

            if (intervalStart != default)
                start = intervalStart;

            if (intervalEnd != default)
                end = intervalEnd;
            
            var query = (from ev in Db.EventLog
                        where ev.IdUser == idUser &&
                              ev.Date >= start &&
                              ev.Date <= end
                        orderby ev.Id
                        select ev);

            if (isUnreadRequested)
                query = query.Where(e => e.IsRead == false)
                    .OrderBy(e => e.Id);

            var entities = await query.ToListAsync(token);
            
            var dtos = entities.Select(Convert);
            return dtos;
        }

        public async Task<int> MarkAsReadAsync(GenericCollectionDto<int> idsDto,
            CancellationToken token)
        {
            var existingEntityDtos = await base.GetExistingEntitiesAsync(idsDto.Collection,
                token);

            var entities = existingEntityDtos.Adapt<List<Event>>();
            
            foreach (var entity in entities)
                entity.IsRead = true;

            Db.EventLog.UpdateRange(entities);
            return await Db.SaveChangesAsync(token);
        }
    }
}