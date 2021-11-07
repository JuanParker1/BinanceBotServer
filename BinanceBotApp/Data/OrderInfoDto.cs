namespace BinanceBotApp.Data
{
    /// <summary>
    /// Order info
    /// </summary>
    public class OrderInfoDto : CreatedOrderResultDto
    {
        /// <summary>
        /// Coin stop price
        /// </summary>
        public double StopPrice { get; set; }
        public double IcebergQty { get; set; }
        /// <summary>
        /// Order time
        /// </summary>
        public long Time { get; set; }
        public long UpdateTime { get; set; }
        public double OrigQuoteOrderQty { get; set; }
    }
}