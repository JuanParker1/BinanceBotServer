using Microsoft.EntityFrameworkCore;
using BinanceBotDb.Models;

namespace DevDemoOperations;

public class DbDemoDataService
{
    public static void AddDemoData()
    {
        Console.WriteLine("Заполнить БД тестовыми данными? y/n");
        var result = Console.ReadLine();

        if (result != "y")
        {
            Console.WriteLine("Хорошо, в другой раз.");
            return;
        }

        try
        {
            var options = new DbContextOptionsBuilder<BinanceBotDbContext>()
                .UseNpgsql("Host=localhost;Database=binanceBotDb;Username=postgres;Password=q;Persist Security Info=True;")
                .Options;
            using var demoContext = new BinanceBotDbContext(options);

            demoContext.Users.AddRange(
                new User
                {
                    Id = 1,
                    IdRole = 1,
                    Login = "dev",
                    Password =
                        "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91", // dev
                    Name = "Developer",
                    DateCreated = DateTime.Parse("2022-06-08T12:01:19.000000")
                }
            );
                
            demoContext.UserSettings.AddRange(
                new Settings
                {
                    Id = 1,
                    IdUser = 1,
                    IsTradeEnabled = false,
                    IdTradeMode = 1,
                    LimitOrderRate = 25
                }
            );
            
            demoContext.BalanceChanges.AddRange(
                new BalanceChange
                {
                    Id = 1,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-02 15:30"),
                    IdDirection = 1,
                    Coin = "USDT",
                    Amount = 100
                },
                new BalanceChange
                {
                    Id = 2,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-03 15:30"),
                    IdDirection = 1,
                    Coin = "BTC",
                    Amount = 400
                },
                new BalanceChange
                {
                    Id = 3,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-03 15:30"),
                    IdDirection = 2,
                    Coin = "ETC",
                    Amount = 200
                },
                new BalanceChange
                {
                    Id = 4,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-04 15:30"),
                    IdDirection = 1,
                    Coin = "USDT",
                    Amount = 1000
                },
                new BalanceChange
                {
                    Id = 5,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-05 15:30"),
                    IdDirection = 2,
                    Coin = "ETH",
                    Amount = 500
                }
            );
            
            demoContext.EventLog.AddRange(
                new Event
                {
                    Id = 1,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-05 15:30"),
                    IsRead = false,
                    Text = "Совершена покупка торговой пары BTC/USDT в количестве 10 шт. по курсу 1000 USDT " +
                           "на сумму 10000 USDT. Дата: 05.02.2022 Время: 15:30."
                },
                new Event
                {
                    Id = 2,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-05 16:49"),
                    IsRead = false,
                    Text = "Совершена покупка торговой пары LTC/USDT в количестве 2 шт. по курсу 500 USDT " +
                           "на сумму 1000 USDT. Дата: 05.02.2022 Время: 16:49."
                },
                new Event
                {
                    Id = 3,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-06 07:02"),
                    IsRead = false,
                    Text = "Совершена продажа торговой пары BTC/USDT в количестве 2 шт. по курсу 1100 USDT " +
                           "на сумму 2200 USDT. Дата: 06.02.2022 Время: 07:02."
                },
                new Event
                {
                    Id = 4,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-07 11:41"),
                    IsRead = false,
                    Text = "Произошла ошибка при попытке покупки торговой пары ETC/USDT в количестве 20 шт. " +
                           "по курсу 52 USDT на сумму 1040 USDT. Дата: 07.02.2022 Время: 11:41.\n" +
                           "Текст ошибки: Unable to place order. One of request params was invalid.",
                },
                new Event
                {
                    Id = 5,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-08 17:42"),
                    IsRead = false,
                    Text = "Совершена покупка торговой пары ETH/USDT в количестве 110 шт. по курсу 532 USDT " +
                           "на сумму 58520 USDT. Дата: 08.02.2022 Время: 17:42."
                }
            );
            
            demoContext.Orders.AddRange(
                new Order
                {
                    Id = 1,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 10,
                    DateCreated = DateTime.Parse("2022-02-05 15:30"),
                    DateClosed = DateTime.Parse("2022-02-06 11:20"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 10,
                    Price = 100
                },
                new Order
                {
                    Id = 2,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 11,
                    DateCreated = DateTime.Parse("2022-02-06 11:20"),
                    DateClosed = DateTime.Parse("2022-02-06 15:30"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 1,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 7,
                    Price = 110
                },
                new Order
                {
                    Id = 3,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 12,
                    DateCreated = DateTime.Parse("2022-02-06 15:30"),
                    DateClosed = DateTime.Parse("2022-02-07 14:30"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 1,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 11,
                    Price = 90
                },
                new Order
                {
                    Id = 4,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 13,
                    DateCreated = DateTime.Parse("2022-02-07 14:30"),
                    DateClosed = DateTime.Parse("2022-02-07 15:30"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 8,
                    Price = 120
                },
                new Order
                {
                    Id = 5,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 14,
                    DateCreated = DateTime.Parse("2022-02-07 15:30"),
                    DateClosed = DateTime.Parse("2022-02-10 15:30"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 10,
                    Price = 100
                },
                new Order
                {
                    Id = 6,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 15,
                    DateCreated = DateTime.Parse("2022-02-10 15:30"),
                    DateClosed = DateTime.Parse("2022-02-11 15:30"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 20,
                    Price = 78
                },
                new Order
                {
                    Id = 7,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 16,
                    DateCreated = DateTime.Parse("2022-02-11 15:30"),
                    DateClosed = DateTime.Parse("2022-02-12 11:45"),
                    IdOrderStatus = 3,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 11,
                    Price = 90
                },
                new Order
                {
                    Id = 8,
                    IdUser = 1,
                    ClientOrderId = "some id",
                    OrderId = 17,
                    DateCreated = DateTime.Parse("2022-02-12 11:45"),
                    DateClosed = null,
                    IdOrderStatus = 1,
                    Symbol = "BNBUSDT",
                    IdSide = 2,
                    IdType = 1,
                    IdCreationType = 1,
                    TimeInForce = "GTC",
                    Quantity = 12,
                    Price = 110
                }
            );
                
            var res = demoContext.SaveChanges();

            Console.WriteLine(res > 0 
                ? "Даннные добавлены" 
                : "Ошибка при добавлении данных");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException?.InnerException?.Message 
                              ?? ex.InnerException?.Message 
                              ?? ex.Message);
            Console.ReadLine();
        }
    }
}