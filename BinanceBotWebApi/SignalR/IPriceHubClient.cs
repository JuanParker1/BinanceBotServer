namespace BinanceBotWebApi.SignalR
{
    public interface IPriceHubClient
    {
        string GetPricesAsync(string price);
    }
}