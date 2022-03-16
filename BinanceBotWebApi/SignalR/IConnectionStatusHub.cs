namespace BinanceBotWebApi.SignalR
{
    public interface IConnectionStatusHub
    {
        string GetStatusAsync(int idUser);
    }
}