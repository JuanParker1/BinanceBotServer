using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> SignedGetRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> PostRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> SignedPostRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams, CancellationToken token );

        Task<HttpResponseMessage> PutRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> SignedPutRequestAsync(Uri endpoint, 
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> DeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams, CancellationToken token);

        Task<HttpResponseMessage> SignedDeleteRequestAsync(Uri endpoint,
            IDictionary<string, string> qParams, CancellationToken token);
    }
}