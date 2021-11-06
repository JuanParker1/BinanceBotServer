using System;

namespace BinanceBotApp.DataInternal.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Convert a DateTime object to Unix time representation (in seconds).
        /// </summary>
        /// <param name="date">The datetime object to convert to Unix timestamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static long ConvertToUnixTimeSeconds(this DateTime date)
        {
            var startTime = new DateTime(1970, 1, 1, 
                0, 0, 0, DateTimeKind.Utc);

            return (long)(date - startTime).TotalSeconds;
        }
        
        /// <summary>
        /// Convert a DateTime object to Unix time representation (in milliseconds).
        /// </summary>
        /// <param name="date">The datetime object to convert to Unix timestamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static long ConvertToUnixTimeMilliseconds(this DateTime date)
        {
            var startTime = new DateTime(1970, 1, 1, 
                0, 0, 0, DateTimeKind.Utc);

            return (long)(date - startTime).TotalMilliseconds;
        }
    }
}