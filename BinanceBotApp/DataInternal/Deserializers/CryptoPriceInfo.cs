namespace BinanceBotApp.DataInternal.Deserializers
{
    public class CryptoPriceInfo
    {
        public long Time { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public double VolumeFrom { get; set; }
        public double VolumeTo { get; set; }
        public double Close { get; set; }
        public string ConversionType { get; set; }
        public string ConversionSymbol { get; set; }
    }
}