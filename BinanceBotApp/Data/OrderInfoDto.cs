namespace BinanceBotApp.Data
{
    public class OrderInfoDto : CreatedOrderResultDto
    {
        public double StopPrice { get; set; }
        public double IcebergQty { get; set; }
        public long Time { get; set; }
        public long UpdateTime { get; set; }
        public double OrigQuoteOrderQty { get; set; }
    }
}