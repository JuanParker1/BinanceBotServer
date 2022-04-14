using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class RequestTrackerControllerTests
{
    private readonly Mock<IRequestTrackerService> _requestService;
    private readonly RequestTrackerController _controller;

    public RequestTrackerControllerTests()
    {
        _requestService = new Mock<IRequestTrackerService>();

        _controller = new RequestTrackerController(_requestService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_requests_if_wrong_user_id()
    {
        var result = _controller.GetRequestsAsync(0, 10)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_requests_if_dtos_is_not_null()
    {
        _requestService.Setup(s => s.GetUserRequestsAsync(It.IsAny<int>(), 
                It.IsAny<int>(), CancellationToken.None).Result)
            .Returns(new List<RequestDto>());

        var result = _controller.GetRequestsAsync(1, 10)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_requests_if_dtos_is_null()
    {
        _requestService.Setup(s => s.GetUserRequestsAsync(It.IsAny<int>(), 
                It.IsAny<int>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<RequestDto>>(null));

        var result = _controller.GetRequestsAsync(1, 10)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
}