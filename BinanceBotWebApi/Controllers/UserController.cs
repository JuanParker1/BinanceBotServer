using System;
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
        /// Gets user info
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPut("userInfo")]
        [ProducesResponseType(typeof(AuthUserInfoDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserInfoAsync([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var result = await _userService.GetUserInfoAsync(idUser, token);

            return Ok(result);
        }
        
        /// <summary>
        /// Updates user info
        /// </summary>
        /// <param name="authUserDto"> User info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPut("userInfo")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserInfoAsync(AuthUserInfoDto authUserDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != authUserDto.Id)
                return Forbid();
            
            var result = await _userService.UpdateUserInfoAsync(authUserDto, token);

            return Ok(result);
        }

        /// <summary>
        /// Gets user exchange stream data
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="listenKey"> User listen key </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User data </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("data")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserDataStreamAsync([Range(1, int.MaxValue)] int idUser, 
            [StringLength(50)] string listenKey, CancellationToken token = default)
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
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("subscriptions")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetSubscriptionsListAsync([Range(1, int.MaxValue)] int idUser, 
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