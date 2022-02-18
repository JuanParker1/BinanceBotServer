using System;

namespace BinanceBotApp.Data
{
    public class BalanceChangeDto : IId
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Direction { get; set; }
        public string Coin { get; set; }
        public double Amount { get; set; }
    }
}