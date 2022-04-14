namespace BinanceBotApp.Data.Analytics
{
    /// <summary>
    /// Order types/profit ratio to all orders amount  
    /// </summary>
    public class TradeTypesStatsDto
    {
        /// <summary>
        /// Rate of auto created orders by third-party signals
        /// </summary>
        public double SignalOrdersRate { get; set; }
        
        /// <summary>
        /// Rate of orders, closed by auto created stop_loss order
        /// </summary>
        public double StopOrdersRate { get; set; }
        
        /// <summary>
        /// Rate of orders, created and closed manually
        /// </summary>
        public double ManualOrdersRate { get; set; }
        
        /// <summary>
        /// Rate of profit from orders by third-party signals
        /// </summary>
        public double SignalOrdersProfit { get; set; }
        
        /// <summary>
        /// Rate of profit from orders, closed by auto created stop_loss order
        /// </summary>
        public double StopOrdersProfit { get; set; }
        
        /// <summary>
        /// Rate of profit from orders, created and closed manually
        /// </summary>
        public double ManualOrdersProfit { get; set; }
    }
}