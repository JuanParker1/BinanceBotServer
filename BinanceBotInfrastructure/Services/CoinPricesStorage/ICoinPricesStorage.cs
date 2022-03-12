namespace BinanceBotInfrastructure.Services.CoinPricesStorage
{
    public interface ICoinPricesStorage
    {
        double? GetCurrentHighestPrice(int idUser, string tradePair);
        void UpdateHighestPrice(int idUser, string tradePair, 
            double newPrice);
    }
}