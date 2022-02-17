using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(int idUser,
            bool isUnreadRequested, int days, CancellationToken token);
        Task<int> MarkAsReadAsync(GenericCollectionDto<int> idsDto,
            CancellationToken token);
    }
}