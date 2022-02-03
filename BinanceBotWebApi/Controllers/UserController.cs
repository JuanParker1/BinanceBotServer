using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// User info controller
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICoinService _coinService;

        public UserController(IUserService userService, ICoinService coinService)
        {
            _userService = userService;
            _coinService = coinService;
        }
        
        /// <summary>
        /// Updates user info
        /// </summary>
        /// <param name="userDto"> User info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserInfoAsync(UserBaseDto userDto, 
            CancellationToken token = default)
        {
            var idUser = User.GetUserId();

            if (idUser is null || idUser != userDto.Id)
                return Forbid();
            
            var result = await _userService.UpdateUserInfoAsync(userDto, token);

            return Ok(result);
        }
        
        /// <summary>
        /// Saves user's Binance api keys
        /// </summary>
        /// <param name="apiKeysDto"> Api keys info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpPost("apiKeys")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SaveApiKeysAsync(ApiKeysDto apiKeysDto, 
            CancellationToken token = default)
        {
            var idUser = User.GetUserId();

            if (idUser is null || idUser != apiKeysDto.IdUser)
                return Forbid();
            
            var result = await _userService.SaveApiKeysAsync(apiKeysDto, token)
                .ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// Gets user data
        /// </summary>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> User data </returns>
        [HttpGet("data")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserDataStreamAsync(string listenKey, 
            CancellationToken token = default)
        {
            await _userService.GetUserDataStreamAsync(listenKey, 
                Console.WriteLine, token).ConfigureAwait(false);
            
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
            await _coinService.GetSubscriptionsListAsync(token)// TODO: Id user проверять везде!!!
                .ConfigureAwait(false);
            
            return Ok();
        }
    }
}