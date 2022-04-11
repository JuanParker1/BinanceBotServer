using System;

namespace BinanceBotApp.Data.Analytics
{
    /// <summary>
    /// Detailed profit info by order types for every requested time interval  
    /// </summary>
    public class ProfitDetailsDto
    {
        /// <summary>
        /// Requested date  
        /// </summary>
        public DateTime Date { get; set; }
        
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