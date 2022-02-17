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
            CancellationToken token)
        {
            var entities = await (from ev in Db.EventLog
                            where ev.IdUser == idUser
                            orderby ev.Id
                            select ev).ToListAsync(token);
            
            var dtos = entities.Select(Convert);
            return dtos;
        }
    }
}