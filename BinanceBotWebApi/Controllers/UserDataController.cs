using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Data;
using BinanceBotApp.Services;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// User info controller
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataService _userDataService;
        private readonly ICoinService _coinService;

        public UserDataController(IUserDataService userDataService,
            ICoinService coinService)
        {
            _userDataService = userDataService;
            _coinService = coinService;
        }
        
        /// <summary>
        /// Gets user data streams listen key
        /// </summary>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> User data streams listen key </returns>
        [HttpPost("listenKey")]
        [ProducesResponseType(typeof(ListenKeyDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetListenKeyAsync(CancellationToken token = default)
        {
            var response = await _userDataService.GetListenKey(token);
            
            return Ok(response);
        }
        
        /// <summary>
        /// Extends user data streams listen key
        /// </summary>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpPut("listenKey")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> ExtendListenKeyAsync(string listenKey, 
            CancellationToken token = default)
        {
            await _userDataService.ExtendListenKey(listenKey, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Deletes user data streams listen key
        /// </summary>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpDelete("listenKey")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteListenKeyAsync(string listenKey, 
            CancellationToken token = default)
        {
            await _userDataService.DeleteListenKey(listenKey, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets user data
        /// </summary>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> User data </returns>
        [HttpGet("data")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SubscribeForStreamAsync(string listenKey, 
            CancellationToken token = default)
        {
            await _userDataService.SubscribeForStreamAsync(listenKey, 
                Console.WriteLine, token);
            
            return Ok();
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
    }
}