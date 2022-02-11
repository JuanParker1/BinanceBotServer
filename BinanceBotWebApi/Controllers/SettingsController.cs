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
        /// Gets user application settings
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(SettingsDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetUserSettings([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var settingsDto = await _settingsService.GetSettingsAsync(idUser, token);

            return Ok(settingsDto);
        }
        
        /// <summary>
        /// Enables/disables trade
        /// </summary>
        /// <param name="enableTradeDto"> Enable/Disable trade info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("enableTrade")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> EnableTradeAsync([FromBody] EnableTradeDto enableTradeDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != enableTradeDto.IdUser)
                return Forbid();

            var result = await _settingsService.EnableTradeAsync(enableTradeDto, 
                token);

            return Ok(result);
        }
        
        /// <summary>
        /// Changes trade mode
        /// </summary>
        /// <param name="tradeModeDto"> Trademode info (1-AutoTrade, 2-Semiauto) </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("tradeMode")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SaveTradeModeAsync([FromBody] TradeModeDto tradeModeDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != tradeModeDto.IdUser)
                return Forbid();

            var result = await _settingsService.SaveTradeModeAsync(tradeModeDto, 
                token);

            return Ok(result);
        }
        
        /// <summary>
        /// Changes limit auto order price rate
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="orderPriceRate"> Limit auto order rate in percents from highest price </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("orderPriceRate")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> ChangeOrderPriceRateAsync([Range(1, int.MaxValue)] int idUser, 
            [Range(10, 30)] int orderPriceRate, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();

            var result = await _settingsService.ChangeOrderPriceRateAsync(idUser, orderPriceRate, 
                token);

            return Ok(result);
        }
        
        /// <summary>
        /// Saves user's Binance api keys
        /// </summary>
        /// <param name="apiKeysDto"> Api keys info object </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> 0 - no changes. 1 - changes applied </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpPost("apiKeys")]
        [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> SaveApiKeysAsync(ApiKeysDto apiKeysDto, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != apiKeysDto.IdUser)
                return Forbid();
            
            var result = await _settingsService.SaveApiKeysAsync(apiKeysDto, 
                token);

            return Ok(result);
        }
    }
}