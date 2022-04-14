using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.DataInternal.Enums;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;
using BinanceBotWebApi.SignalR;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Coins info controller
    /// </summary>
    [Route("api/coins")]
    [ApiController]
    [Authorize]
    public class CoinController : ControllerBase
    {
        private readonly ICoinService _coinService;
        private readonly IHubContext<PricesHub> _pricesHubContext;

        public CoinController(ICoinService coinService, IHubContext<PricesHub> pricesHubContext)
        {
            _coinService = coinService;
            _pricesHubContext = pricesHubContext;
        }
        
        /// <summary>
        /// Gets list of all available trading pairs
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> List of all trading pairs </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("tradingPairs")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetTradingPairsAsync([FromQuery][Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var allPairs = await _coinService.GetTradingPairsAsync(idUser, 
                token);
            
            return Ok(allPairs);
        }
        
        /// <summary>
        /// Gets price info for requested trading pair in real time
        /// (in single coin stream)
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("price/{pair}")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetSingleCoinPriceStreamAsync([FromRoute][StringLength(20)] string pair, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            async void HandleSelectedCoinPriceAsync(string price) =>
                await _pricesHubContext.Clients.Group($"User_{authUserId}_Price").SendAsync(
                    nameof(IPricesHubClient.GetPriceAsync),
                    price,
                    token
                );
            
            await _coinService.SubscribeSingleCoinPriceStreamAsync(pair, idUser, 
                HandleSelectedCoinPriceAsync, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets price info for requested trading pairs in real time
        /// (in multi coin stream)
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pairNames"> Trading pairs names </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("price")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCoinsPricesStreamAsync([FromQuery] GenericCollectionDto<string> pairNames, 
            [Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            async void HandleCoinPricesAsync(string price) =>
                await _pricesHubContext.Clients.Group($"User_{authUserId}").SendAsync(
                    nameof(IPricesHubClient.GetPricesAsync),
                    price,
                    token
                );
            
            await _coinService.SubscribeCoinPricesStreamAsync(pairNames.Collection, idUser, 
                HandleCoinPricesAsync, token);

            return Ok();
        }
        
        /// <summary>
        /// Stops receiving info for requested pair (in single coin stream)
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete("price/{pair}")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UnsubscribeSingleCoinPriceStreamAsync([FromRoute] string pair, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.UnsubscribeCoinPriceStreamAsync(new List<string> {pair}, 
                idUser, WebsocketConnectionTypes.Price, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Stops receiving info for requested pairs (in multi coin stream)
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pairNames"> Trading pairs names </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete("prices")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UnsubscribeCoinPricesStreamAsync([FromQuery] GenericCollectionDto<string> pairNames, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.UnsubscribeCoinPriceStreamAsync(pairNames.Collection, 
                idUser, WebsocketConnectionTypes.Prices, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets list of coins under price monitoring (in multi coin stream)
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubscriptionsListAsync([FromQuery][Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            await _coinService.GetSubscriptionsListAsync(idUser, token);

            return Ok();
        }
    }
}