namespace BinanceBotInfrastructure.Utils
{
    public static class Verifier
    {
        public static bool IsNumericDefault(object value)
        {
            var intVal = 1;
            var doubleVal = 1.0;

            return (int.TryParse($"{value}", out intVal) || 
                    double.TryParse($"{value}", out doubleVal)) && 
                   (intVal == default || doubleVal == default);
        }
    }
}