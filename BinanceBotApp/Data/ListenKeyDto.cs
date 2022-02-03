namespace BinanceBotApp.Data
{
    /// <summary>
    /// Listen key request
    /// </summary>
    public class ListenKeyDto // TODO: А зачем dto? слушать api будет только бэк
    {
        /// <summary>
        /// Listen key for request
        /// </summary>
        public string ListenKey { get; set; }
    }
}