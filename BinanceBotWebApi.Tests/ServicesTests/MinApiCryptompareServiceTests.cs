using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotInfrastructure.Services;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ServicesTests;

public class MinApiCryptompareServiceTests
{
    private readonly Mock<IHttpClientService> _httpService;
    private readonly MinApiCryptoCompareService _service;

    public MinApiCryptompareServiceTests()
    {
        _httpService = new Mock<IHttpClientService>();
        
        _httpService.Setup(s => s.GetRequestAsync<PriceApiResponse>(It.IsAny<string>(), 
                CancellationToken.None).Result)
            .Returns(new PriceApiResponse()
            {
                Data = new CryptoPrice()
                {
                    Data = new List<CryptoPriceInfo>
                    {
                        new () {Time = 1650026005, High = 43100},
                        new () {Time = 1650026105, High = 43150}
                    }
                }
            });
        
        _service = new MinApiCryptoCompareService(_httpService.Object);
    }
    
    [Fact]
    public async void It_should_return_correct_data_in_get_price_history()
    {
        var entites = await _service.GetPriceHistoryAsync("", 
            DateTime.MinValue, DateTime.MaxValue, CancellationToken.None);
        
        Assert.Equal(2, entites.Count());
    }
}