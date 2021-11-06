namespace BinanceBotApp.Data
{
    public class CreatedOrderResultDto : CreatedOrderAckDto
    {
        public string Price { get; set; }
        public string OrigQty { get; set; }
        public string ExecuteQty { get; set; }
        public string CummulativeQuoteQty { get; set; }
        public string Status { get; set; }
        public string TimeInForce { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
    }
}