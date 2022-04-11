using System;

namespace BinanceBotInfrastructure.Extensions
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
        
        /// <summary>
        /// Convert Unix time representation (in seconds) to DateTime object.
        /// </summary>
        /// <param name="date">The datetime object to convert to Unix timestamp.</param>
        /// <param name="timestamp">Unix timestamp to create DateTime object from.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        public static DateTime FromUnixTimeSeconds(this DateTime date, long timestamp)
        {
            var startTime = new DateTime(1970, 1, 1, 
                0, 0, 0, DateTimeKind.Utc);

            return startTime.AddSeconds(timestamp);
        }
    }
}