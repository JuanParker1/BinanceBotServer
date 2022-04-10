using System.Collections.Generic;

namespace BinanceBotApp.DataInternal.Deserializers
{
    public class CryptoPrice
    {
        public bool Aggregated { get; set; }
        public long TimeFrom { get; set; }
        public long TimeTo { get; set; }
        public IEnumerable<CryptoPriceInfo> Data { get; set; }
    }
}