namespace BinanceBotApp.DataInternal.Deserializers
{
    public class PriceApiResponse
    {
        public string Response { get; set; }
        public string Message { get; set; }
        public bool HasWarning { get; set; }
        public int Type { get; set; }
        public CryptoPrice Data { get; set; }
    }
}