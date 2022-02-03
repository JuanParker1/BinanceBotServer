using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Services;

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
        /// Gets list of all trading pairs
        /// </summary>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> List of all trading pairs </returns>
        [HttpGet("tradingPairs")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetTradingPairsAsync(CancellationToken token = default)
        {
            var allPairs = await _coinService.GetTradingPairsAsync(token)
                .ConfigureAwait(false);
            return Ok(allPairs);
        }

        /// <summary>
        /// Gets price info for requested trading pair in real time
        /// </summary>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pair in real time </returns>
        [HttpGet("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCoinPriceStreamAsync([FromRoute]string pair, 
            CancellationToken token = default)
        {
            await _coinService.GetCoinPriceStreamAsync(pair, Console.WriteLine, token)
                .ConfigureAwait(false);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets price info for requested trading pairs in real time
        /// </summary>
        /// <param name="pairNames"> Trading pairs names </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pairs in real time </returns>
        [HttpGet("combined/info")]
        [ProducesResponseType(typeof(OrderInfoDto), (int)System.Net.HttpStatusCode.OK)] //http://localhost:5000/api/coins/combined/info?collection=ethbtc&collection=btcusdt
        public async Task<IActionResult> GetCoinsListPriceStreamAsync([FromQuery]GenericCollectionDto<string> pairNames,
            CancellationToken token = default)
        {
            await _coinService.GetCoinsListPriceStreamAsync(pairNames, Console.WriteLine, token)
                .ConfigureAwait(false);
        
            return Ok();
        }
        
        /// <summary>
        /// Stops receiving info for requested pair
        /// </summary>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpDelete("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UnsubscribeCoinPriceStreamAsync([FromRoute]string pair, 
            CancellationToken token = default)
        {
            await _coinService.UnsubscribeCoinPriceStreamAsync(pair, token)
                .ConfigureAwait(false);
            
            return Ok();
        }
    }
}