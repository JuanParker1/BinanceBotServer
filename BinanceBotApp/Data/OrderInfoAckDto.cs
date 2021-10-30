namespace BinanceBotApp.Data
{
    public class OrderInfoAckDto
    {
        public string Symbol { get; set; }
        public int OrderId { get; set; }
        public int OrderListId { get; set; }
        public string ClientOrderId { get; set; }
        public long TransactTime { get; set; }
    }
}