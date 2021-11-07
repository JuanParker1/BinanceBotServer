namespace BinanceBotApp.Data
{
    /// <summary>
    /// The smallest new order info object
    /// </summary>
    public class CreatedOrderAckDto
    {
        /// <summary>
        /// Trading pair
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// Order id
        /// </summary>
        public int OrderId { get; set; }
        public int OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public long TransactTime { get; set; }
    }
}