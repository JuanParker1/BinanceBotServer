using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Services;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// User info retrieving controller
    /// </summary>
    [Route("api/user")]
    [ApiController]
    //[Authorize]
    public class UserDataController : ControllerBase
    {
        private readonly IUserDataService _userDataService;

        public UserDataController(IUserDataService userDataService)
        {
            _userDataService = userDataService;
        }
        
        /// <summary>
        /// Gets user data streams listen key
        /// </summary>
        /// <param name="token"> Task cancellation token </param>
        /// <returns> User data streams listen key </returns>
        [HttpPost("listenKey")]
        [ProducesResponseType(typeof(string), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetListenKeyAsync(CancellationToken token = default)
        {
            var listenKey = await _userDataService.GetListenKey(token);
            
            return Ok(listenKey);
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
    }
}