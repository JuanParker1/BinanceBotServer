namespace BinanceBotApp.DataInternal.Deserializers
{
    /// <summary>
    /// The smallest new order info object
    /// </summary>
    public class CreatedOrderAck
    {
        /// <summary>
        /// Trading pair
        /// </summary>
        public string Symbol { get; set; }
        
        /// <summary>
        /// Order id
        /// </summary>
        public long OrderId { get; set; }
        public long OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public long TransactTime { get; set; }
    }
}