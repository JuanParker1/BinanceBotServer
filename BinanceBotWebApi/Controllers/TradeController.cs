using System.Collections.Generic;
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
        /// Creates new test order (without sending it into the matching engine)
        /// </summary>
        /// <param name="orderParams"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> New test order info </returns>
        [HttpPost("test")]
        [ProducesResponseType(typeof(OrderInfoAckDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTestOrderAsync([FromBody]OrderParamsDto orderParams, 
            CancellationToken token = default)
        {
            var orderInfo = await _tradeService.CreateTestOrderAsync(orderParams, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="orderParams"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> New order info </returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderInfoAckDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateOrderAsync([FromBody]OrderParamsDto orderParams, 
            CancellationToken token = default)
        {
            var orderInfo = await _tradeService.CreateOrderAsync(orderParams, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Deletes requested order
        /// </summary>
        /// <param name="idOrder"> Requested order id </param>
        /// <param name="symbol"> Requested trade pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Info about cancelled order </returns>
        [HttpDelete]
        [ProducesResponseType(typeof(DeletedOrderInfoDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteOrderAsync(int idOrder, string symbol,
            int recvWindow = 5000, CancellationToken token = default)
        {
            var orderInfo = await _tradeService.DeleteOrderAsync(idOrder, symbol, 
                recvWindow, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Deletes all orders for requested trading pair
        /// </summary>
        /// <param name="symbol"> Requested trade pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Info about cancelled order </returns>
        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<DeletedOrderInfoDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAllOrderForPairAsync(string symbol,
            int recvWindow = 5000, CancellationToken token = default)
        {
            var orderInfo = 
                await _tradeService.DeleteAllOrdersForPairAsync(symbol, 
                    recvWindow, token);

            return Ok(orderInfo);
        }
    }
}