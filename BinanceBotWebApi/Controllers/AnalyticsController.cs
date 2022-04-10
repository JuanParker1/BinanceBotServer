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
        /// Gets balance change data
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's total balance change info </returns> // TODO: Вот тут точно в контроллерах косяк
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("balanceChange")]
        [ProducesResponseType(typeof(BalanceChangeDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetBalanceChangeAsync([Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            // var historyDtos = await _balanceService.GetAllAsync(idUser, 
            //     intervalStart, intervalEnd, token);

            return Ok();
        }
    }
}