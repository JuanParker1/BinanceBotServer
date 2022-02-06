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
        /// <param name="isTradeEnabled"> User info object </param>
        /// <param name="id"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns></returns>
        [HttpPost("enableTrade")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateUserInfoAsync(bool isTradeEnabled, int id, 
            CancellationToken token = default)
        {
            var idUser = User.GetUserId();

            if (idUser is null || idUser != id)
                return Forbid();

            var result = await _settingsService.EnableTradeAsync(id, isTradeEnabled, token);

            return Ok(result);
        }
    }
}