using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Контроллер управления ордерами
    /// </summary>
    [Route("api/trade")]
    [ApiController]
    //[Authorize]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }
        
        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="orderParams"> New order params </param>
        /// <param name="token"> Токен отмены задачи </param>
        /// <returns> New order info </returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderInfoAckDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateOrderAsync(OrderParamsDto orderParams, 
            CancellationToken token = default)
        {
            var orderInfo = await _tradeService.CreateOrderAsync(orderParams, token);

            return Ok(orderInfo);
        }
    }
}