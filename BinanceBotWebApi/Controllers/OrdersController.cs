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
    /// Orders info controller
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        
        /// <summary>
        /// Gets info about requested order
        /// </summary>
        /// <param name="idOrder"> Requested order id </param>
        /// <param name="symbol"> Requested trading pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Info about requested order </returns>
        [HttpGet("{idOrder}")]
        [ProducesResponseType(typeof(OrderInfoDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderAsync([FromRoute]int idOrder, [FromQuery]string symbol, 
            int recvWindow = 5000,  CancellationToken token = default)
        {
            var orderInfo = await _ordersService.GetOrderAsync(idOrder, symbol, 
                recvWindow, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Gets all orders for requested symbol info
        /// </summary>
        /// <param name="symbol"> Requested trading pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Info about requested orders for trading pair </returns>
        [HttpGet("{symbol}")]
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersForPairAsync([FromRoute]string symbol, 
            int recvWindow = 5000,  CancellationToken token = default)
        {
            var ordersInfo = await _ordersService.GetOrdersForPairAsync(symbol, 
                recvWindow, token);

            return Ok(ordersInfo);
        }
        
        /// <summary>
        /// Gets all opened orders info
        /// </summary>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> All opened orders info </returns>
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersForPairAsync(int recvWindow = 5000,  
            CancellationToken token = default)
        {
            var ordersInfo = await _ordersService.GetAllOrdersAsync(recvWindow, token);

            return Ok(ordersInfo);
        }
        
        /// <summary>
        /// Creates new test order (without sending it into the matching engine)
        /// </summary>
        /// <param name="createOrder"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> New test order info </returns>
        [HttpPost("test")]
        [ProducesResponseType(typeof(CreatedOrderAckDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTestOrderAsync([FromBody]CreateOrderDto createOrder, 
            CancellationToken token = default)
        {
            var orderInfo = await _ordersService.CreateTestOrderAsync(createOrder, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="createOrder"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> New order info </returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedOrderAckDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateOrderAsync([FromBody]CreateOrderDto createOrder, 
            CancellationToken token = default)
        {
            var orderInfo = await _ordersService.CreateOrderAsync(createOrder, token);

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
        [HttpDelete("{idOrder}")]
        [ProducesResponseType(typeof(DeletedOrderDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute]int idOrder, string symbol,
            int recvWindow = 5000, CancellationToken token = default)
        {
            var orderInfo = await _ordersService.DeleteOrderAsync(idOrder, symbol, 
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
        [ProducesResponseType(typeof(IEnumerable<DeletedOrderDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAllOrderForPairAsync(string symbol,
            int recvWindow = 5000, CancellationToken token = default)
        {
            var orderInfo = 
                await _ordersService.DeleteAllOrdersForPairAsync(symbol, 
                    recvWindow, token);

            return Ok(orderInfo);
        }
    }
}