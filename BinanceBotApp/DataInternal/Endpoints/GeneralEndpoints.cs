using System;

namespace BinanceBotApp.DataInternal.Endpoints
{
    /// <summary>
    /// General endpoints info.
    /// </summary>
    public static class GeneralEndpoints
    {
        /// <summary>
        /// Get current status of the API (on/maintenance).
        /// Weight: unknown.
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetSystemStatusEndpoint() =>
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/system/status");

        /// <summary>
        /// Test connectivity to the API. Weight: 1.
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetTestConnectivityEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/ping");

        /// <summary>
        /// Test connectivity to the API and get the current
        /// server time. Weight: 1.
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetServerTimeEndpoint() =>
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/time");
    }
}