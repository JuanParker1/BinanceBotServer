using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceBotApp.Services
{
    public interface IHttpResponseService
    {
        Task<T> HandleResponseAsync<T>(HttpResponseMessage message, 
            CancellationToken token) where T : class;
    }
}