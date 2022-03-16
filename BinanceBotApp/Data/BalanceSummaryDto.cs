namespace BinanceBotApp.Data
{
    /// <summary>
    /// Total account balance info
    /// </summary>
    public class BalanceSummaryDto
    {
        /// <summary>
        /// Total deposit sum
        /// </summary>
        public double? TotalDeposit { get; set; }
        
        /// <summary>
        /// Total withdraw sum
        /// </summary>
        public double? TotalWithdraw { get; set; }
    }
}