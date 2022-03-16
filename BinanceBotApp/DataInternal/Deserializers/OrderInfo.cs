namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// Order info
    /// </summary>
    public class OrderInfo : CreatedOrderResult
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