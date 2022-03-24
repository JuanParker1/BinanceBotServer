using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class FixedEventTemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 1,
                column: "template",
                value: "Создан ордер на {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT на сумму {4} USDT. \n Способ: {5}. \n  Дата: {6}");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 2,
                column: "template",
                value: "Произошла ошибка при создании ордера на {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT на сумму {4} USDT. \n Текст ошибки: {5} \n Способ: {6}. \n Дата: {7}");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "template",
                value: "Совершена {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT на сумму {4} USDT. \n Ордер выполнен. \n Дата: {5}");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 4,
                column: "template",
                value: "Произошла отмена ордера на {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT на сумму {4} USDT. \n Способ: {5} Дата: {6}");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 5,
                column: "template",
                value: "Произошла ошибка при отмене ордера на {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT на сумму {4} USDT. \n Текст ошибки: {5} \n Способ: {6}. \n Дата: {7}");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 6,
                column: "template",
                value: "Произошла ошибка при чтении ответа от биржи при {0} торговой пары {1} в количестве {2} шт. по курсу {3} USDT. \n Текст ошибки: {4} \n Способ: {5}. \n Дата: {6}");

            migrationBuilder.InsertData(
                table: "t_event_templates",
                columns: new[] { "id", "template" },
                values: new object[,]
                {
                    { 7, "Автоматическая торговля {0}. \n Дата: {1}" },
                    { 8, "Запрошена продажа всей криптовалюты на аккаунте. \n Дата: {0}" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 1,
                column: "template",
                value: "Совершена покупка торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 2,
                column: "template",
                value: "Совершена продажа торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "template",
                value: "На бирже установлен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 4,
                column: "template",
                value: "На бирже отменен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 5,
                column: "template",
                value: "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 6,
                column: "template",
                value: "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}.");
        }
    }
}
