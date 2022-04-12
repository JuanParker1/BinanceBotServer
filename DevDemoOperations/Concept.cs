namespace DevDemoOperations;

// All code that can be used later is stored here
public class Concept
{
    // WebSocket listen key is used to Listen User data streams.
    
    // public async Task<string> GetListenKey(CancellationToken token)
    // {
    //     var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
    //     
    //     var listenKey = await _httpService.ProcessRequestAsync<string>(uri,
    //         null, default, HttpMethods.SignedPost, token);
    //     
    //     return listenKey;
    // }
    //     
    // public async Task ExtendListenKey(string listenKey, CancellationToken token)
    // {
    //     var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
    //     
    //     await _httpService.ProcessRequestAsync<string>(uri,
    //         new Dictionary<string, string>(), 
    //         default, HttpMethods.SignedPut, token);
    // }
    //     
    // public async Task DeleteListenKey(string listenKey, CancellationToken token)
    // {
    //     var uri = UserDataWebSocketEndpoints.GetListenKeyEndpoint();
    //     
    //     await _httpService.ProcessRequestAsync<string>(uri,
    //         new Dictionary<string, string>(), 
    //         default, HttpMethods.SignedDelete, token);
    // }
}