using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Application settings controller
    /// </summary>
    [Route("api/settings")]
    [ApiController]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        
        /// <summary>
        /// Enables/disables trade
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="isTradeEnabled"> User info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("enableTrade")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserInfoAsync(bool isTradeEnabled, int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var result = await _settingsService.EnableTradeAsync(idUser, isTradeEnabled, token);

            return Ok(result);
        }
    }
}