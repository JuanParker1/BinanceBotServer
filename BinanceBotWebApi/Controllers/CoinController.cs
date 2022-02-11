using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

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

        public CoinController(ICoinService coinService)
        {
            _coinService = coinService;
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
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCoinPriceStreamAsync([FromRoute][StringLength(20)] string pair, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.GetCoinPriceStreamAsync(pair, Console.WriteLine, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets price info for requested trading pairs in real time
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pairNames"> Trading pairs names </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("combinedInfo")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)] //http://localhost:5000/api/coins/combined/info?collection=ethbtc&collection=btcusdt
        public async Task<IActionResult> GetCoinsListPriceStreamAsync([FromQuery] GenericCollectionDto<string> pairNames, 
            [Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.GetCoinsListPriceStreamAsync(pairNames, Console.WriteLine, token);
        
            return Ok();
        }
        
        /// <summary>
        /// Stops receiving info for requested pair
        /// </summary>
        /// <param name="idUser"> Requested user id </param>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Ok </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpDelete("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UnsubscribeCoinPriceStreamAsync([FromRoute][StringLength(20)] string pair, 
            [FromQuery][Range(1, int.MaxValue)] int idUser, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.UnsubscribeCoinPriceStreamAsync(pair, token);
            
            return Ok();
        }
    }
}