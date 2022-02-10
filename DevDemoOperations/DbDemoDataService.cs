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
                        "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91",
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
                    Amount = 100
                },
                new BalanceChange
                {
                    Id = 2,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-03 15:30"),
                    IdDirection = 1,
                    Amount = 400
                },
                new BalanceChange
                {
                    Id = 3,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-03 15:30"),
                    IdDirection = 2,
                    Amount = 200
                },
                new BalanceChange
                {
                    Id = 4,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-04 15:30"),
                    IdDirection = 1,
                    Amount = 1000
                },
                new BalanceChange
                {
                    Id = 5,
                    IdUser = 1,
                    Date = DateTime.Parse("2022-02-05 15:30"),
                    IdDirection = 2,
                    Amount = 500
                }
            );
                
            var res = demoContext.SaveChanges();

            Console.WriteLine(res > 0 
                ? "Даннные добавлены" 
                : "Ошибка при добавлении данных");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }
}