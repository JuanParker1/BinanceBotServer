using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Extensions;

namespace BinanceBotWebApi.Controllers;

/// <summary>
/// Cryptocurrency info controller from third-party api
/// </summary>
[Route("api/cryptoInfo")]
[ApiController]
[Authorize]
public class CryptoInfoController : ControllerBase
{
    private readonly ICryptoInfoService _cryptoInfoService;

    public CryptoInfoController(ICryptoInfoService cryptoInfoService)
    {
        _cryptoInfoService = cryptoInfoService;
    }
    
    /// <summary>
    /// Gets cryptocurrency price history for time interval
    /// </summary>
    /// <param name="idUser"> Requested user id </param>
    /// <param name="symbol"> Cryptocurrency name </param>
    /// <param name="intervalStart"> Interval start date </param>
    /// <param name="intervalEnd"> Interval end date </param>
    /// <param name="token"> Task cancellation token </param>
    /// <returns code="200"> Cryptocurrency price history for time interval </returns>
    /// <response code="400"> Error in request parameters </response>
    /// <response code="403"> Wrong user id </response>
    [HttpGet("priceHistory")]
    [ProducesResponseType(typeof(OrderInfo), (int)System.Net.HttpStatusCode.OK)]
    public async Task<IActionResult> GetBtcPriceHistoryAsync([Range(1, int.MaxValue)]int idUser, 
        [MaxLength(5)]string symbol, DateTime intervalStart, DateTime intervalEnd, 
        CancellationToken token = default)
    {
        var authUserId = User.GetUserId();

        if (authUserId is null || authUserId != idUser)
            return Forbid();
            
        var orderInfo = await _cryptoInfoService.GetPriceHistoryAsync(symbol, 
            intervalStart, intervalEnd, token);

        return Ok(orderInfo);
    }
}