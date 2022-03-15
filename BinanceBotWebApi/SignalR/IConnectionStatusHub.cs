namespace BinanceBotWebApi.SignalR
{
    public interface IConnectionStatusHub
    {
        string GetStatus(int idUser);
    }
}