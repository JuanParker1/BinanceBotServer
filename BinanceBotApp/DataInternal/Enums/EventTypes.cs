namespace BinanceBotApp.DataInternal.Enums
{
    public enum EventTypes
    {
        OrderCreated,
        OrderCreationError,
        OrderFilled,
        OrderCancelled,
        OrderCancellationError,
        UnknownDataReceived,
        TradeSwitched,
        AllCoinsSold
    }
}