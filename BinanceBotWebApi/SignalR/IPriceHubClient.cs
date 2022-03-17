namespace BinanceBotWebApi.SignalR
{
    public interface IPriceHubClient
    {
        string GetPriceAsync(string price);
        string GetPricesAsync(string price);
    }
}