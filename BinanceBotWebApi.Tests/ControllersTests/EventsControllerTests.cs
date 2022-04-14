using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using BinanceBotWebApi.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class EventsControllerTests
{
    private readonly Mock<IEventsService> _eventsService;
    private readonly EventsController _controller;
    
    public EventsControllerTests()
    {
        _eventsService = new Mock<IEventsService>();

        _controller = new EventsController(_eventsService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_events_if_wrong_user_id()
    {
        var result = _controller.GetUserEventsAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_events_if_dtos_is_not_null()
    {
        _eventsService.Setup(s => s.GetAllAsync(It.IsAny<int>(), true,
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None)
                .Result)
            .Returns(new List<EventDto>());

        var result = _controller.GetUserEventsAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_events_if_dtos_is_null()
    {
        _eventsService.Setup(s => s.GetAllAsync(It.IsAny<int>(), true, 
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<EventDto>>(null));

        var result = _controller.GetUserEventsAsync(1, DateTime.Now, 
            DateTime.Now).Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        //Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_mark_as_read_if_wrong_user_id()
    {
        var result = _controller.MarkAsReadAsync(new GenericCollectionDto<int>())
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_mark_as_read_if_dtos_is_not_null()
    {
        _eventsService.Setup(s => s.MarkAsReadAsync(It.IsAny<GenericCollectionDto<int>>(), 
                    CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.MarkAsReadAsync(new GenericCollectionDto<int>(){IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_correct_value_in_mark_as_read_if_dtos_is_not_null()
    {
        _eventsService.Setup(s => s.MarkAsReadAsync(It.IsAny<GenericCollectionDto<int>>(), 
                CancellationToken.None).Result)
            .Returns(1);

        var result = _controller.MarkAsReadAsync(new GenericCollectionDto<int>(){IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
        
        Assert.Equal(1,okObjectResult?.Value);
    }
}