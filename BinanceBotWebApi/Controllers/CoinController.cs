using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services;

namespace BinanceBotWebApi.Controllers
{
    /// <summary>
    /// Контроллер сбора информации о монетах на бирже
    /// </summary>
    [Route("api/coins")]
    [ApiController]
    [Authorize]
    public class CoinController : ControllerBase
    {
        private readonly ICoinService _coinService;
        
        public CoinController(ICoinService coinService)
        {
            _coinService = coinService;
        }
        
        /// <summary>
        /// Возвращает список всех торговых пар на бирже
        /// </summary>
        /// <param name="token">Токен для отмены задачи</param>
        /// <returns>Список всех доступных торговых пар</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), (int)System.Net.HttpStatusCode.OK)]
        public async Task<IEnumerable<string>> GetAllAsync(
            CancellationToken token = default)
        {
            var allPairs = await _coinService.GetAllAsync(token);
            return allPairs;
        }
        
        /// <summary>
        /// Возвращает текущую информацию о торговой паре
        /// </summary>
        /// <param name="symbol">Запрашиваемая торговая пара</param>
        /// <param name="token">Токен для отмены задачи</param>
        /// <returns>Информацию по запрашиваемой торговой паре</returns>
        [HttpGet("{symbol}/info")]
        [ProducesResponseType(typeof(CoinBestAskBidDto), (int)System.Net.HttpStatusCode.OK)]
        public async Task<CoinBestAskBidDto> GetInfoAsync([FromRoute]string symbol, 
            CancellationToken token = default)
        {
            var coinInfo = await _coinService.GetBestPriceAsync(symbol, token);
            return coinInfo;
        }
        
        // /// <summary>
        // /// Возвращает информацию о торговой паре за период
        // /// </summary>
        // /// <param name="pair">Запрашиваемая торговая пара</param>
        // /// <param name="token">Токен для отмены задачи</param>
        // /// <returns>Информацию о торговой паре за период</returns>
        // [HttpGet("{pair}/history")]
        // [ProducesResponseType(typeof(int), (int)System.Net.HttpStatusCode.OK)]
        // public async Task<int> GetHistoryAsync(string pair, CancellationToken token = default)
        // {
        //     return 1;
        // }
    }
}