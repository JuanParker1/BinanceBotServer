using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BinanceBotInfrastructure.Services.CoinPricesStorage
{
    /// <summary>
    /// Keeps highest prices for trading pair in user's wallet
    /// </summary>
    public class CoinPricesStorage : ICoinPricesStorage
    {
        private readonly ConcurrentDictionary<int, IDictionary<string, double>> _coinPricesStorage;

        public CoinPricesStorage()
        {
            _coinPricesStorage = new ConcurrentDictionary<int, IDictionary<string, double>>();
        }
        
        public double? GetCurrentHighestPrice(int idUser, string tradePair)
        {
            var highestCoinPrices = new Dictionary<string, double>();
            var highestPrices = _coinPricesStorage.GetOrAdd(idUser, highestCoinPrices);
            if(highestPrices.ContainsKey(tradePair))
                return highestPrices[tradePair];

            return null;
        }

        public void UpdateHighestPrice(int idUser, string tradePair, double newPrice)
        {
            var highestCoinPrices = new Dictionary<string, double>();
            var highestPrices = _coinPricesStorage.GetOrAdd(idUser, highestCoinPrices);
            highestPrices[tradePair] = newPrice;
        }
    }
}