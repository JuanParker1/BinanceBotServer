using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services.Cache;
using BinanceBotDb.Models;
using Mapster;


namespace BinanceBotInfrastructure.Services
{
    public class RequestTrackerService : IRequestTrackerService
    {
        private static readonly char[] _stackTraceSeparators = "\r\n".ToCharArray();
        private static readonly int _requestObsolescenceHours = 2;

        private readonly CacheTable<Request> _cacheRequestLogs;

        public RequestTrackerService(BinanceBotDbContext db, CacheDb cacheDb)
        {
            _cacheRequestLogs = cacheDb.GetCachedTable<Request>((BinanceBotDbContext) db);
        }

        public async Task<IEnumerable<RequestDto>> GetUserRequestsAsync(int idUser,
            int take, CancellationToken token = default)
        {
            var requests = await _cacheRequestLogs.WhereAsync(r => 
                r.IdUser == idUser, token);

            if (requests.Any() && take > 0)
                requests = requests.Take(take);

            var requestDtos = requests.Adapt<IEnumerable<RequestDto>>();

            return requestDtos;
        }

        public async Task RegisterRequestAsync(RequestDto requestDto, 
            CancellationToken token = default)
        {
            if (requestDto.Status < 200 || !await IsNewRequestAsync(requestDto, token))
                return;
            
            requestDto.Date = DateTime.Now;
            var request = requestDto.Adapt<Request>();
            await _cacheRequestLogs.InsertAsync(request, token);
        }

        public async Task RegisterRequestErrorAsync(RequestDto requestDto, Exception ex,
            CancellationToken token)
        {
            if (!await IsNewErrorRequest(requestDto, token))
                return;
            
            requestDto.Date = DateTime.Now;
            requestDto.ExceptionMessage = ex.InnerException?.InnerException?.Message 
                                          ?? ex.InnerException?.Message 
                                          ?? ex.Message;
            requestDto.ExceptionStack = ex.StackTrace?.Split(_stackTraceSeparators)[0];
            var request = requestDto.Adapt<Request>();
            
            await _cacheRequestLogs.InsertAsync(request, token);
        }
        
        private async Task<bool> IsNewRequestAsync(RequestDto requestDto,
            CancellationToken token)
        {
            var lastRequest = await _cacheRequestLogs.LastOrDefaultAsync(r => 
                r.IdUser == requestDto.IdUser, token);

            if (lastRequest is null)
                return true;

            var isNewRequest = (requestDto.Date - lastRequest.Date).Hours > 
                             _requestObsolescenceHours;
            var isNewIp = requestDto.Ip != lastRequest.Ip;

            return isNewRequest || isNewIp;
        }

        private async Task<bool> IsNewErrorRequest(RequestDto requestDto,
            CancellationToken token)
        {
            var lastRequest = await _cacheRequestLogs.LastOrDefaultAsync(r => 
                r.IdUser == requestDto.IdUser, token);
            
            if (lastRequest is null)
                return true;

            return requestDto.Status != lastRequest.Status;
        }
    }
}