using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

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
        /// <param name="idUser"> Requested user id </param>
        /// <param name="idOrder"> Requested order id </param>
        /// <param name="pair"> Requested trading pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about requested order </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("{idOrder}")]
        [ProducesResponseType(typeof(OrderInfo), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderAsync([FromRoute][Range(1, int.MaxValue)] int idOrder, 
            [FromQuery][StringLength(20)] string pair, [Range(1, int.MaxValue)]int idUser, 
            [Range(5000, int.MaxValue)]int recvWindow = 5000, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var orderInfo = await _ordersService.GetOrderAsync(idUser, idOrder, 
                pair, recvWindow, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Gets all orders for requested symbol info
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Requested trading pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about requested orders for trading pair </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("{symbol}")]
        [ProducesResponseType(typeof(IEnumerable<OrderInfo>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersForPairAsync([FromRoute][StringLength(20)] string pair, // TODO: Оставить для api, но на фронте может быть не нужно
            [FromQuery][Range(1, int.MaxValue)] int idUser, [Range(5000, int.MaxValue)] int recvWindow = 5000,  
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var ordersInfo = await _ordersService.GetOrdersForPairAsync(idUser, 
                pair, recvWindow, token);

            return Ok(ordersInfo);
        }
        
        /// <summary>
        /// Gets all opened orders info for requested user
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> All opened orders info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveOrdersAsync([FromQuery][Range(1, int.MaxValue)] int idUser,  
            [Range(5000, int.MaxValue)] int recvWindow = 5000, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var ordersInfo = await _ordersService.GetActiveOrdersAsync(idUser, 
                recvWindow, token);

            return Ok(ordersInfo);
        }

        /// <summary>
        /// Gets all orders history for requested symbol in time interval
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="symbol"> Requested trading pair </param>
        /// <param name="days"> Requested interval in days (0 value returns
        /// all events for all time) </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about requested orders for trading pair </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("{symbol}/history")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersHistoryForPairAsync([FromRoute][StringLength(20), MinLength(2)] string symbol, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, [Range(0, int.MaxValue)] int days = 1, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser ||
                symbol.ToUpper().StartsWith("USDT"))
                return Forbid();
            
            var ordersInfo = await _ordersService.GetOrdersHistoryForPairAsync(idUser, 
                symbol, days, token);

            return Ok(ordersInfo);
        }
        
        /// <summary>
        /// Gets all orders history for requested user in time interval
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about requested orders for trading pair </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersHistoryAsync([FromQuery][Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart = default, DateTime intervalEnd = default, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var ordersInfo = await _ordersService.GetOrdersHistoryAsync(idUser, intervalStart, 
                intervalEnd, token);

            return Ok(ordersInfo);
        }

        /// <summary>
        /// Creates new test order (without sending it into the matching engine)
        /// </summary>
        /// <param name="newOrderDto"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> New test order info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("test")]
        [ProducesResponseType(typeof(CreatedOrderFull), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateTestOrderAsync([FromBody] NewOrderDto newOrderDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != newOrderDto.IdUser)
                return Forbid();
            
            var orderInfo = await _ordersService.CreateTestOrderAsync(newOrderDto, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Creates new order
        /// </summary>
        /// <param name="newOrder"> New order params </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> New order info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] NewOrderDto newOrder, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != newOrder.IdUser)
                return Forbid();
            
            await _ordersService.CreateOrderAsync(newOrder, token);

            return Ok();
        }
        
        /// <summary>
        /// Sell all coins at user account
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <returns code="200"> Info about cancelled order </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("sellAll")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)] // TODO: Решить вопрос с ошибкой Бинанса Timestamp is out of range. Возможнор, увеличить в контроллерах дефолтный recWindow
        public IActionResult SellAllCoins([FromBody][Range(1, int.MaxValue)] int idUser, 
            [Range(5000, int.MaxValue)] int recvWindow = 5000)
        {
            var authUserId = User.GetUserId();
        
            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            _ordersService.SellAllCoins(idUser, recvWindow);
        
            return Ok();
        }
        
        /// <summary>
        /// Deletes requested order
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="idOrder"> Requested order id (first variant)</param>
        /// <param name="clientOrderId"> Requested order id (second variant) </param>
        /// <param name="symbol"> Requested trade pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about cancelled order </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete]
        [ProducesResponseType(typeof(DeletedOrder), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteOrderAsync([FromQuery][Range(1, int.MaxValue)] int idUser,
            [Range(1, long.MaxValue)] long idOrder, [StringLength(30)] string clientOrderId,  
            [StringLength(20)] string symbol, [Range(5000, int.MaxValue)] int recvWindow = 5000, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var orderInfo = await _ordersService.DeleteOrderAsync(idUser, 
                idOrder, clientOrderId, symbol.ToUpper(), recvWindow, 
                token);

            return Ok(orderInfo);
        }
    }
}