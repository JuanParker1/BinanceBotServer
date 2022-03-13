using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BinanceBotInfrastructure.Services.CoinPricesStorage
{
    /// <summary>
    /// Keeps highest prices for trading pairs in user's wallet
    /// </summary>
    public class CoinPricesStorage : ICoinPricesStorage
    {
        private readonly ConcurrentDictionary<int, IDictionary<string, double>> _coinPricesStorage;

        public CoinPricesStorage()
        {
            _coinPricesStorage = new ConcurrentDictionary<int, IDictionary<string, double>>();
        }

        public IDictionary<string, double> Get(int idUser) =>
            _coinPricesStorage.GetOrAdd(idUser, new Dictionary<string, double>());

        public bool Remove(int idUser) =>
            _coinPricesStorage.Remove(idUser, out var result);
    }
}