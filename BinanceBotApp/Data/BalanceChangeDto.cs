using System;

namespace BinanceBotApp.Data
{
    public class BalanceChangeDto : IId
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public DateTime Date { get; set; }
        public int IdDirection { get; set; }
        public double Amount { get; set; }
    }
}