using System;

namespace BinanceBotApp.DataInternal.Endpoints
{
    // The base API endpoint is in GeneralInfo.cs -> ApiBaseUrl
    // A User Data Stream listenKey is valid for 60 minutes after creation.
    // Doing a PUT on a listenKey will extend its validity for 60 minutes.
    // Doing a DELETE on a listenKey will close the stream and invalidate the listenKey.
    // Doing a POST on an account with an active listenKey will return the currently
    // active listenKey and extend its validity for 60 minutes.
    // The base websocket endpoint is: wss://stream.binance.com:9443
    // User Data Streams are accessed at /ws/<listenKey> or /stream?streams=<listenKey>
    // A single connection to stream.binance.com is only valid for 24 hours; expect to be disconnected at the 24 hour mark
    
    public static class UserDataWebSocketEndpoints
    {
        /// <summary>
        /// Sets the Binance Listen Key. Send POST request
        /// to start user stream. Weight: 1 for all requests.
        /// POST request returns listen key (string).
        /// PUT request with {listenKey} param will extend it's
        /// validity for next 60 minutes (Recommended to refresh it
        /// about every 30 minutes).
        /// DELETE request with {listenKey} param will close
        /// a user data stream.
        /// </summary>
        /// <returns>User data Websocket uri</returns>
        public static Uri GetUserDataWebSocketEndpoint()
        {
            var requestUrl = $"{GeneralInfo.ApiBaseUrl}/" +
                             $"{GeneralInfo.ApiVersion3}/userDataStream";

            return new Uri(requestUrl);
        }
    }
}