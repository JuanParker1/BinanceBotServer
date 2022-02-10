namespace BinanceBotApp.Data
{
    public class SettingsDto
    {
        public bool IsTradeEnabled { get; set; }
        public string TradeMode { get; set; }
        public int LimitOrderRate { get; set; }
        public bool IsApiKeysSet { get; set; }
        public double TotalDeposit { get; set; } // TODO: Это явно должно быть не тут. Это из UserController. На фронте тоже поправить.
        public double TotalWithdraw { get; set; }
    }
}