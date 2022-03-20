namespace BinanceBotInfrastructure.Utils
{
    public static class Verifier
    {
        public static bool IsNumericDefault(object value) =>
            (int.TryParse($"{value}", out var intVal) | 
                    double.TryParse($"{value}", out var doubleVal)) && 
                   intVal == default && doubleVal == default;
    }
}