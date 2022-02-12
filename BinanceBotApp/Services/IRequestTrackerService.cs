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
            int take = -1, CancellationToken token = default);
        Task RegisterRequestAsync(RequestDto requestDto, 
            CancellationToken token = default);
        Task RegisterRequestErrorAsync(RequestDto requestDto, Exception ex,
            CancellationToken token = default);
    }
}