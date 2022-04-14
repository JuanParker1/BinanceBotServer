using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BinanceBotApp.Data;
using BinanceBotApp.DataInternal.Deserializers;
using BinanceBotApp.Services;
using BinanceBotWebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BinanceBotWebApi.Tests.ControllersTests;

public class OrdersControllerTests
{
    private readonly Mock<IOrdersService> _ordersService;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _ordersService = new Mock<IOrdersService>();

        _controller = new OrdersController(_ordersService.Object);

        _controller.AddUser();
    }
    
    [Fact]
    public void It_should_return_403_in_get_order_if_wrong_user_id()
    {
        var result = _controller.GetOrderAsync(0, "" , 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_order_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.GetOrderAsync(It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new OrderInfo());

        var result = _controller.GetOrderAsync(0, "" , 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_order_if_dto_is_null()
    {
        _ordersService.Setup(s => s.GetOrderAsync(It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), 
                CancellationToken.None))
            .Returns(Task.FromResult<OrderInfo>(null));

        var result = _controller.GetOrderAsync(0, "" , 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_orders_for_pair_if_wrong_user_id()
    {
        var result = _controller.GetOrdersForPairAsync("" , 0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_orders_for_pair_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.GetOrdersForPairAsync(It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), 
                CancellationToken.None).Result)
            .Returns(new List<OrderInfo>());

        var result = _controller.GetOrdersForPairAsync("" , 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_orders_for_pair_if_dto_is_null()
    {
        _ordersService.Setup(s => s.GetOrdersForPairAsync(It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), 
                CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<OrderInfo>>(null));

        var result = _controller.GetOrdersForPairAsync("" , 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_active_orders_if_wrong_user_id()
    {
        var result = _controller.GetActiveOrdersAsync(0)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_active_orders_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.GetActiveOrdersAsync(It.IsAny<int>(), 
                It.IsAny<int>(), CancellationToken.None).Result)
            .Returns(new List<OrderDto>());

        var result = _controller.GetActiveOrdersAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_active_orders_if_dto_is_null()
    {
        _ordersService.Setup(s => s.GetActiveOrdersAsync(It.IsAny<int>(), 
                It.IsAny<int>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<OrderDto>>(null));

        var result = _controller.GetActiveOrdersAsync(1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_orders_history_for_pair_if_wrong_user_id()
    {
        var result = _controller.GetOrdersHistoryForPairAsync("", 0, 1)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_orders_history_for_pair_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.GetOrdersHistoryForPairAsync(It.IsAny<int>(), 
                It.IsAny<string>(),It.IsAny<int>(), CancellationToken.None).Result)
            .Returns(new List<OrderDto>());

        var result = _controller.GetOrdersHistoryForPairAsync("", 1, 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_orders_history_for_pair_if_dto_is_null()
    {
        _ordersService.Setup(s => s.GetOrdersHistoryForPairAsync(It.IsAny<int>(), 
                It.IsAny<string>(),It.IsAny<int>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<OrderDto>>(null));

        var result = _controller.GetOrdersHistoryForPairAsync("", 1, 1)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_get_orders_history_if_wrong_user_id()
    {
        var result = _controller.GetOrdersHistoryAsync(0, DateTime.Now, DateTime.Now)
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_get_orders_history_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.GetOrdersHistoryAsync(It.IsAny<int>(), 
                It.IsAny<DateTime>(),It.IsAny<DateTime>(), CancellationToken.None).Result)
            .Returns(new List<OrderDto>());

        var result = _controller.GetOrdersHistoryAsync(1, DateTime.Now, DateTime.Now)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_get_orders_history_if_dto_is_null()
    {
        _ordersService.Setup(s => s.GetOrdersHistoryAsync(It.IsAny<int>(), 
                It.IsAny<DateTime>(),It.IsAny<DateTime>(), CancellationToken.None))
            .Returns(Task.FromResult<IEnumerable<OrderDto>>(null));

        var result = _controller.GetOrdersHistoryAsync(1, DateTime.Now, DateTime.Now)
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_create_test_order_if_wrong_user_id()
    {
        var result = _controller.CreateTestOrderAsync(new NewOrderDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_create_test_order_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.CreateTestOrderAsync(It.IsAny<NewOrderDto>(), 
                CancellationToken.None).Result)
            .Returns(new CreatedOrderFull());

        var result = _controller.CreateTestOrderAsync(new NewOrderDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_204_in_create_test_order_if_dto_is_null()
    {
        _ordersService.Setup(s => s.CreateTestOrderAsync(It.IsAny<NewOrderDto>(), 
                 CancellationToken.None))
            .Returns(Task.FromResult<CreatedOrderResult>(null));

        var result = _controller.CreateTestOrderAsync(new NewOrderDto {IdUser = 1})
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
    
    [Fact]
    public void It_should_return_403_in_create_order_if_wrong_user_id()
    {
        var result = _controller.CreateOrderAsync(new NewOrderDto {IdUser = 0})
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_create_order()
    {
        var result = _controller.CreateOrderAsync(new NewOrderDto {IdUser = 1})
            .Result;
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_sell_all_coins_if_wrong_user_id()
    {
        var result = _controller.SellAllCoins(0);
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_sell_all_coins()
    {
        var result = _controller.SellAllCoins(1);
        var okResult = result as OkResult;
    
        Assert.NotNull(okResult);
    }
    
    [Fact]
    public void It_should_return_403_in_delete_order_if_wrong_user_id()
    {
        var result = _controller.DeleteOrderAsync(0, 0, "", "")
            .Result;
        var forbidResult = result as ForbidResult;
    
        Assert.NotNull(forbidResult);
    }
    
    [Fact]
    public void It_should_return_ok_result_in_delete_order_if_dto_is_not_null()
    {
        _ordersService.Setup(s => s.DeleteOrderAsync(It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), CancellationToken.None).Result)
            .Returns(new DeletedOrder());

        var result = _controller.DeleteOrderAsync(1, 0, "", "")
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public void It_should_return_204_in_delete_order_if_dto_is_null()
    {
        _ordersService.Setup(s => s.DeleteOrderAsync(It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(),CancellationToken.None))
            .Returns(Task.FromResult<DeletedOrder>(null));

        var result = _controller.DeleteOrderAsync(1, 0, "", "")
            .Result;
        var okObjectResult = result as OkObjectResult;
    
        Assert.NotNull(okObjectResult);
        Assert.Null(okObjectResult?.Value);
    }
}