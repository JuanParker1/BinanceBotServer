using System;

namespace BinanceBotApp.Data
{
    public class OrderDto : NewOrderDto
    {
        /// <summary>
        /// Order id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Order creation date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Auto or manually created order
        /// </summary>
        public string CreationType { get; set; }
        
        /// <summary>
        /// Coin price for the moment when order was created
        /// </summary>
        public double CoinPrice { get; set; }
    }
}