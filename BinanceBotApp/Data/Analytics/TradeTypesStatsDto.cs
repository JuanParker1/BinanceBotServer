namespace BinanceBotApp.Data.Analytics
{
    public class TradeTypesStatsDto
    {
        public double SignalOrdersRate { get; set; }
        public double StopOrdersRate { get; set; }
        public double ManualOrdersRate { get; set; }
        public double SignalsProfit { get; set; }
        public double StopOrdersProfit { get; set; }
        public double ManualOrdersProfit { get; set; }
    }
}