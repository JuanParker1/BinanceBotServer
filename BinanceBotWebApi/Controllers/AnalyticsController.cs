using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.Data.Analytics;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Trade analytics controller
    /// </summary>
    [Route("api/analytics")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Gets profit rate to btc price for time interval
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Profit rate to btc price for time interval </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("profitToBtc")]
        [ProducesResponseType(typeof(ProfitToBtcDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetProfitToBtcAsync([Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var profitToBtcDtos = await _analyticsService.GetProfitToBtcAsync(idUser, 
                intervalStart, intervalEnd, token);

            return Ok(profitToBtcDtos);
        }
        
        /// <summary>
        /// Gets trade types data (orders rate, their profit rate, etc)
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> Trade types data </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("tradeTypesStats")]
        [ProducesResponseType(typeof(TradeTypesStatsDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetTradeTypesStatsAsync([Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var tradeTypesStatsDtos = await _analyticsService.GetTradeTypesStatsAsync(idUser, 
                intervalStart, intervalEnd, token);

            return Ok(tradeTypesStatsDtos);
        }
    }
}