using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(int idUser, 
            bool isUnreadRequested, DateTime intervalStart, DateTime intervalEnd, 
            CancellationToken token);
        Task<string> CreateEventTextAsync(EventTypes eventType,
            IEnumerable<string> eventParams, CancellationToken token);
        Task<int> CreateEventAsync(int idUser, string eventText,
            CancellationToken token);
        Task<int> MarkAsReadAsync(GenericCollectionDto<int> idsDto,
            CancellationToken token);
    }
}