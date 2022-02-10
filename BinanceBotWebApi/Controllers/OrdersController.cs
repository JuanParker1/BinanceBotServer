using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.Data;
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
        [ProducesResponseType(typeof(OrderInfoDto), (int)System.Net.HttpStatusCode.OK)]
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
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersForPairAsync([FromRoute][StringLength(20)] string pair, 
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
        /// Gets all opened orders info
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> All opened orders info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderInfoDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery][Range(1, int.MaxValue)] int idUser,  
            [Range(5000, int.MaxValue)] int recvWindow = 5000, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var ordersInfo = await _ordersService.GetAllOrdersAsync(idUser, 
                recvWindow, token);

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
        [ProducesResponseType(typeof(CreatedOrderResultDto), (int)System.Net.HttpStatusCode.OK)]
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
        [ProducesResponseType(typeof(CreatedOrderResultDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> CreateOrderAsync([FromBody] NewOrderDto newOrder, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != newOrder.IdUser)
                return Forbid();
            
            var orderInfo = await _ordersService.CreateOrderAsync(newOrder, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Deletes requested order
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="idOrder"> Requested order id </param>
        /// <param name="symbol"> Requested trade pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about cancelled order </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete("{idOrder}")]
        [ProducesResponseType(typeof(DeletedOrderDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute][Range(1, int.MaxValue)] int idOrder, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, [StringLength(20)] string symbol, 
            [Range(5000, int.MaxValue)] int recvWindow = 5000, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var orderInfo = await _ordersService.DeleteOrderAsync(idUser, 
                idOrder, symbol, recvWindow, token);

            return Ok(orderInfo);
        }
        
        /// <summary>
        /// Deletes all orders for requested trading pair
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Requested trade pair </param>
        /// <param name="recvWindow"> Request lifetime in ms </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Info about cancelled order </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<DeletedOrderDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAllOrderForPairAsync([FromQuery][Range(1, int.MaxValue)] int idUser, 
            [StringLength(20)] string pair, [Range(5000, int.MaxValue)] int recvWindow = 5000, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var orderInfo = 
                await _ordersService.DeleteAllOrdersForPairAsync(idUser, pair, 
                    recvWindow, token);

            return Ok(orderInfo);
        }
    }
}