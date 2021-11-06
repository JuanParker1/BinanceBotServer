namespace BinanceBotApp.Data
{
    public class ApiErrorDto
    {
        public int HttpCode { get; set; }
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}