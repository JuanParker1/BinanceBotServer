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
        /// Gets user's current balance info
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's current balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("balance/current")]
        [ProducesResponseType(typeof(double), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrentBalanceAsync([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var currentBalance = await _analyticsService.GetCurrentBalanceAsync(idUser, 
                token);

            return Ok(currentBalance);
        }
        
        /// <summary>
        /// Gets user's total balance info
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's total balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("balance/total")]
        [ProducesResponseType(typeof(TotalBalanceDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetTotalBalanceAsync([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var totalBalance = await _analyticsService.GetTotalBalanceAsync(idUser, 
                token);

            return Ok(totalBalance);
        }
    }
}