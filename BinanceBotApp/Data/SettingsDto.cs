namespace BinanceBotApp.Data
{
    /// <summary>
    /// User application settings info
    /// </summary>
    public class SettingsDto
    {
        /// <summary>
        /// Enable/disable trade
        /// </summary>
        public bool IsTradeEnabled { get; set; }
        
        /// <summary>
        /// Trade mode
        /// </summary>
        public string TradeMode { get; set; }
        
        /// <summary>
        /// Amount of percents from coin highest price to place STOPLOSS order (auto trade)
        /// </summary>
        public int LimitOrderRate { get; set; }
        
        /// <summary>
        /// Are api keys added to settings or not (trade is impossible without them)
        /// </summary>
        public bool IsApiKeysSet { get; set; }
    }
}