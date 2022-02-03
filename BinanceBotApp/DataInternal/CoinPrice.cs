namespace BinanceBotApp.DataInternal
{
    public class CoinPrice
    {
        public string Symbol { get; set; } // TODO: Тоже непонятно зачем. Десериализовать в словать или в тапл?
        public string Price { get; set; }
    }
}