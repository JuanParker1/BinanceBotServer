namespace BinanceBotApp.Data.Enums
{
    public enum OrderType
    {
        Limit, // deffered order. Executes only when (ask/bid) price reaches the established price
        Market, // buy/sell by current market price
        StopLoss,
        StopLossLimit,
        TakeProfit,
        TakeProfitLimit,
        LimitMaker
    }
}