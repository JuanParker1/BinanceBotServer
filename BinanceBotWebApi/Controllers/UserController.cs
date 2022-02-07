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
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="403"> Wrong user id </response>
        [HttpPut("userInfo")]
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
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("apiKeys")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SaveApiKeysAsync(ApiKeysDto apiKeysDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != apiKeysDto.Id)
                return Forbid();
            
            var result = await _userService.SaveApiKeysAsync(apiKeysDto, token);

            return Ok(result);
        }

        /// <summary>
        /// Gets user data
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User data </returns>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("data")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserDataStreamAsync(int idUser, 
            string listenKey, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _userService.GetUserDataStreamAsync(listenKey, 
                Console.WriteLine, token);
            
            return Ok();
        }
        
        /// <summary>
        /// Gets all websocket connections list
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> List with active websocket subscriptions </returns>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubscriptionsListAsync(int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            await _coinService.GetSubscriptionsListAsync(token);
            
            return Ok();
        }
    }
}