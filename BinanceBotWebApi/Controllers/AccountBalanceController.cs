using System.Collections.Generic;
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
    /// Account balance info controller
    /// </summary>
    [Route("api/balance")]
    [ApiController]
    [Authorize]
    public class AccountBalanceController : ControllerBase
    {
        private readonly IAccountBalanceService _balanceService;

        public AccountBalanceController(IAccountBalanceService balanceService)
        {
            _balanceService = balanceService;
        }
        
        /// <summary>
        /// Gets user's deposit history
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="days"> Requested interval in days (0 value returns
        /// all data for all time) </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's total balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(BalanceChangeDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetDepositHistoryAsync([Range(1, int.MaxValue)] int idUser, 
            [Range(0, int.MaxValue)] int days = 1, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var historyDtos = await _balanceService.GetAllAsync(idUser, 
                days, token);

            return Ok(historyDtos);
        }
        
        /// <summary>
        /// Gets user's current balance info
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's current balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("current")]
        [ProducesResponseType(typeof(IEnumerable<CoinAmountDto>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetCurrentBalanceAsync([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var currentBalance = await _balanceService.GetCurrentBalanceAsync(idUser, 
                token);

            return Ok(currentBalance);
        }
        
        /// <summary>
        /// Gets user's total deposits and withdrawals sum
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's total balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(BalanceSummaryDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetTotalBalanceAsync([Range(1, int.MaxValue)] int idUser, 
            CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var totalBalance = await _balanceService.GetTotalBalanceAsync(idUser, 
                token);

            return Ok(totalBalance);
        }
    }
}