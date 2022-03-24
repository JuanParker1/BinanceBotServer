namespace BinanceBotApp.DataInternal.Enums
{
    public enum EventTypes
    {
        OrderCreated,
        OrderCreationError,
        OrderFilled,
        OrderCancelled,
        OrderCancellationError,
        OrderUnknownDataReceived,
        TradeSwitched,
        AllCoinsSold
    }
}