namespace BinanceBotApp.Data
{
    public class ApiErrorDto
    {
        /// <summary>
        /// Error http status code
        /// </summary>
        public int HttpCode { get; set; }
        /// <summary>
        /// Error exchange api code
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// Error message from exchange api
        /// </summary>
        public string Msg { get; set; }
    }
}