using System;

namespace BinanceBotApp.Data
{
    /// <summary>
    /// Account balance change info
    /// </summary>
    public class BalanceChangeDto : IId
    {
        /// <summary>
        /// User id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Balance change date
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Deposit/withdraw
        /// </summary>
        public string Direction { get; set; }
        
        /// <summary>
        /// Coin name
        /// </summary>
        public string Coin { get; set; }
        
        /// <summary>
        /// Coin amount
        /// </summary>
        public double Amount { get; set; }
    }
}