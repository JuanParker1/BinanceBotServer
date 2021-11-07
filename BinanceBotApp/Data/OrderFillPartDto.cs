namespace BinanceBotApp.Data
{
    /// <summary>
    /// Successful order, that filled main one
    /// </summary>
    public class OrderFillPartDto
    {
        /// <summary>
        /// Coin price
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// Coin quantity
        /// </summary>
        public string Qty { get; set; }
        /// <summary>
        /// Comission
        /// </summary>
        public string Comission { get; set; }
        public string ComissionAsset { get; set; }
    }
}