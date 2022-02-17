using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllAsync(int idUser,
            CancellationToken token);
    }
}