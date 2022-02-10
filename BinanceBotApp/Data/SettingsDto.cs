namespace BinanceBotApp.Data
{
    public class SettingsDto
    {
        public bool IsTradeEnabled { get; set; }
        public string TradeMode { get; set; }
        public int LimitOrderRate { get; set; }
        public bool IsApiKeysSet { get; set; }
    }
}