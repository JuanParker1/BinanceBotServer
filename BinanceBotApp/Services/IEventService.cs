using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(int idUser, 
            bool isUnreadRequested, DateTime intervalStart, DateTime intervalEnd, 
            CancellationToken token);
        Task<int> MarkAsReadAsync(GenericCollectionDto<int> idsDto,
            CancellationToken token);
    }
}