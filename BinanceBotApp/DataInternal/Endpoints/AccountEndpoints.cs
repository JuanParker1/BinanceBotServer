using System;

namespace BinanceBotApp.DataInternal.Endpoints
{
    /// <summary>
    /// Account info endpoints.
    /// </summary>
    public static class AccountEndpoints
    {
        /// <summary>
        /// Fetch account status detail. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAccountStatusEndpoint() =>
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/account/status");

        /// <summary>
        /// Get current account information. Weight: 10
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAccountInformationEndpoint() => 
            new ($"{GeneralInfo.ApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion3}/account");

        /// <summary>
        /// Fetch account api trading status detail. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetApiTradingStatusEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/account/apiTradingStatus");

        /// <summary>
        /// Get information of coins (available for deposit
        /// and withdraw) for user. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAccountBalanceInfoEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/capital/config/getall");

        /// <summary>
        /// Get info about one of available coins at account. Weight: 1 
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetAccountCoinsInfoEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/asset/get-funding-asset");

        /// <summary>
        /// Get daily account snapshot whatever it is.
        /// (Response has info about amount of free,
        /// locked user coins etc). Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetDailyAccountSnapshotEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/accountSnapshot");

        /// <summary>
        /// Fetch account deposit history. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetDepositHistoryEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/hisRec");

        /// <summary>
        /// Fetch account withdraw history. Weight: 1
        /// </summary>
        /// <returns>Endpoint data info object</returns>
        public static Uri GetWithdrawHistoryEndpoint() => 
            new ($"{GeneralInfo.SApiBaseUrl}/" +
                 $"{GeneralInfo.ApiVersion1}/history");
    }
}