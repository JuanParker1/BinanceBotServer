using System.Collections.Generic;

namespace BinanceBotInfrastructure.Services.CoinPricesStorage
{
    public interface ICoinPricesStorage
    {
        IDictionary<string, double> Get(int idUser);
        bool Remove(int idUser);
    }
}