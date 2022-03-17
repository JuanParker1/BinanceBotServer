using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;
using BinanceBotWebApi.SignalR;
using Microsoft.AspNetCore.SignalR;

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
        private readonly ICoinService _coinService;
        private readonly IHubContext<PricesHub> _pricesHubContext;

        public AccountBalanceController(IAccountBalanceService balanceService,
            ICoinService coinService, IHubContext<PricesHub> pricesHubContext)
        {
            _balanceService = balanceService;
            _coinService = coinService;
            _pricesHubContext = pricesHubContext;
        }
        
        /// <summary>
        /// Gets user's deposit history
        /// </summary>
        /// <param name="idUser"> User id </param>
        /// <param name="intervalStart"> Requested interval start date </param>
        /// <param name="intervalEnd"> Requested interval end date </param>
        /// <param name="token"> Task cancellation token </param>
        /// <returns code="200"> User's total balance info </returns>
        /// <response code="400"> Error in request parameters </response>
        /// <response code="403"> Wrong user id </response>
        [HttpGet]
        [ProducesResponseType(typeof(BalanceChangeDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IActionResult> GetDepositHistoryAsync([Range(1, int.MaxValue)] int idUser, 
            DateTime intervalStart, DateTime intervalEnd, CancellationToken token = default)
        {
            var authUserId = User.GetUserId();

            if (authUserId is null || authUserId != idUser)
                return Forbid();
            
            var historyDtos = await _balanceService.GetAllAsync(idUser, 
                intervalStart, intervalEnd, token);

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
    
            async void HandleCoinPricesAsync(string price) =>
                await _pricesHubContext.Clients.Group($"User_{authUserId}_Prices").SendAsync(
                    nameof(IPricesHubClient.GetPricesAsync),
                    price,
                    CancellationToken.None
                );
            
            _coinService.SubscribeCoinPricesStream(currentBalance.Select(b => $"{b.Asset}USDT"), 
                idUser, HandleCoinPricesAsync);

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