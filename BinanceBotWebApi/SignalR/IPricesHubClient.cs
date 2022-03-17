namespace BinanceBotWebApi.SignalR
{
    public interface IPricesHubClient
    {
        string GetPriceAsync(string price);
        string GetPricesAsync(string price);
    }
}