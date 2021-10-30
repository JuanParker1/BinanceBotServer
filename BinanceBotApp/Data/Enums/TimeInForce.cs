namespace BinanceBotApp.Data.Enums
{
    public enum TimeInForce
    {
        Gtc, // "Good till cancelled". Order lives until it's cancelled or filled
        Ioc, // "Immediate or cancel". Order fires if only seller's price is suitable and coins amount is enough for buyer's order
        Fok
    }
}