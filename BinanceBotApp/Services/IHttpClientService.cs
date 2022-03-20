using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.DataInternal.Enums;

namespace BinanceBotApp.Services
{
    public interface IHttpClientService
    {
        Task<TResult> ProcessRequestAsync<TDto, TResult>(Uri uri, TDto dto, 
            (string apiKey, string secretKey) keys, IEnumerable<string> paramsToRemove, 
            HttpMethods requestType, CancellationToken token) where TResult : class;
        Task<TResult> ProcessRequestAsync<TResult>(Uri uri, IDictionary<string, string> qParams,
            (string apiKey, string secretKey) keys, HttpMethods requestType, CancellationToken token) 
            where TResult : class;
    }
}