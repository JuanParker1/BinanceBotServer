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
    /// Coin info from exchange retrieving controller
    /// </summary>
    [Route("api/coins")]
    [ApiController]
    //[Authorize]
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IEnumerable<string>> GetAllAsync(CancellationToken token = default)
        {
            var allPairs = await _coinService.GetAllAsync(token);
            return allPairs;
        }
        
        /// <summary>
        /// Gets price info for requested trading pair
        /// </summary>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pair </returns>
        [HttpGet("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetInfoAsync([FromRoute]string pair, 
            CancellationToken token = default)
        {
            await _coinService.GetPairBestPriceAsync(pair, Console.WriteLine, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets price info for requested trading pairs
        /// </summary>
        /// <param name="pairNames"> Trading pairs names </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pairs </returns>
        [HttpGet("combined/info")]
        [ProducesResponseType(typeof(OrderInfoDto), (int)System.Net.HttpStatusCode.OK)] //http://localhost:5000/api/coins/combined/info?collection=ethbtc&collection=btcusdt
        public async Task<IActionResult> ConnectToWebSocketAsync([FromQuery]GenericCollectionDto<string> pairNames,
            CancellationToken token = default)
        {
            await _coinService.GetPairsBestPricesAsync(pairNames, Console.WriteLine, token);
        
            return Ok();
        }
    }
}