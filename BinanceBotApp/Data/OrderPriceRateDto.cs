namespace BinanceBotApp.Data
{
    /// <summary>
    /// Amount of percents from coin highest price to place STOPLOSS order (auto trade)
    /// </summary>
    public class OrderPriceRateDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public int IdUser { get; set; }
        
        /// <summary>
        /// Amount of percents
        /// </summary>
        public int OrderPriceRate { get; set; }
    }
}