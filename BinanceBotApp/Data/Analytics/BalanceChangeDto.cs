using System;

namespace BinanceBotApp.Data.Analytics
{
    /// <summary>
    /// Balance change history and change reason data
    /// </summary>
    public class BalanceChangeDto
    {
        public DateTime Date { get; set; }
        public double Balance { get; set; }
        public double Profit { get; set; }
        public double BalanceIO { get; set; }
    }
}