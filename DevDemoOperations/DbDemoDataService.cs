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
                }
            );
                
            demoContext.UserSettings.AddRange(
                new Settings
                {
                    Id = 1,
                    IdUser = 1,
                    IsTradeEnabled = false,
                    TradeMode = 0,
                    LimitOrderRate = 25
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