using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotDb.Models;

namespace BinanceBotInfrastructure.Services
{
    public class EventService : CrudService<EventDto, Event>, IEventService
    {
        public EventService(IBinanceBotDbContext db) : base(db) { }
        
        public virtual async Task<IEnumerable<EventDto>> GetAllAsync(int idUser, 
            int days, CancellationToken token)
        {
            var startDate = DateTime.MinValue;
            
            if (days > 0)
                startDate = DateTime.Now.AddDays(-days);
            
            var entities = await (from ev in Db.EventLog
                            where ev.IdUser == idUser &&
                                  ev.Date > startDate
                            orderby ev.Id
                            select ev).ToListAsync(token);
            
            var dtos = entities.Select(Convert);
            return dtos;
        }
    }
}