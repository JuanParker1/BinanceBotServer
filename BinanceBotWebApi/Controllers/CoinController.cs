﻿using System;
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
    /// Coin info controller
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
        [HttpGet("tradingPairs")]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IEnumerable<string>> GetTradingPairsAsync(CancellationToken token = default)
        {
            var allPairs = await _coinService.GetTradingPairsAsync(token);
            return allPairs;
        }
        
        /// <summary>
        /// Gets all websocket connections list
        /// </summary>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pair in real time </returns>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubscriptionsListAsync(CancellationToken token = default)
        {
            await _coinService.GetSubscriptionsListAsync(token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets price info for requested trading pair in real time
        /// </summary>
        /// <param name="pair"> Trading pair name </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> Price info for requested trading pair in real time </returns>
        [HttpGet("{pair}/info")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SubscribeForStreamAsync([FromRoute]string pair, 
            CancellationToken token = default)
        {
            await _coinService.SubscribeForStreamAsync(pair, Console.WriteLine, token);
            
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
        public async Task<IActionResult> SubscribeForCombinedStreamAsync([FromQuery]GenericCollectionDto<string> pairNames,
            CancellationToken token = default)
        {
            await _coinService.SubscribeForCombinedStreamAsync(pairNames, Console.WriteLine, token);
        
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
        public async Task<IActionResult> UnsubscribeFromStreamAsync([FromRoute]string pair, 
            CancellationToken token = default)
        {
            await _coinService.UnsubscribeFromStreamAsync(pair, token);
            
            return Ok();
        }
    }
}