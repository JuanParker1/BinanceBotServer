namespace BinanceBotApp.Data
{
    /// <summary>
    /// Middle size new order info object
    /// </summary>
    public class CreatedOrderResultDto : CreatedOrderAckDto
    {
        /// <summary>
        /// New order coin price
        /// </summary>
        public string Price { get; set; }
        public string OrigQty { get; set; }
        public string ExecuteQty { get; set; }
        public string CummulativeQuoteQty { get; set; }
        /// <summary>
        /// New order status
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// New order lifetime
        /// </summary>
        public string TimeInForce { get; set; }
        /// <summary>
        /// New order market type (market/limit)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Order side (buy/sell)
        /// </summary>
        public string Side { get; set; }
    }
}