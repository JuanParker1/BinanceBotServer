using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;

namespace BinanceBotApp.Services
{
    public interface IRequestTrackerService
    {
        Task<IEnumerable<RequestDto>> GetUserRequestsAsync(int idUser,
            int take, CancellationToken token = default);
        Task<int> RegisterRequestAsync(RequestDto requestDto, 
            CancellationToken token = default);
    }
}