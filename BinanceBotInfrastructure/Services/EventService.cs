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
            bool isUnreadRequested, int days, CancellationToken token)
        {
            var startDate = DateTime.MinValue;
            
            if (days > 0)
                startDate = DateTime.Now.AddDays(-days);
            
            var query = (from ev in Db.EventLog
                        where ev.IdUser == idUser &&
                              ev.Date > startDate
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

            foreach (var dto in existingEntityDtos)
                dto.IsRead = true;

            var entities = existingEntityDtos.Adapt<IEnumerable<Event>>();
            
            Db.EventLog.UpdateRange(entities);
            return await Db.SaveChangesAsync(token);
        }
    }
}