namespace BinanceBotWebApi.SignalR
{
    public interface IPriceHubClient
    {
        string GetPrices(string price);
    }
}