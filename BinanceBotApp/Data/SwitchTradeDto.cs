namespace BinanceBotApp.Data
{
    /// <summary>
    /// Enable/disable trade
    /// </summary>
    public class SwitchTradeDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Enable/disable
        /// </summary>
        public bool IsTradeEnabled { get; set; }
    }
}