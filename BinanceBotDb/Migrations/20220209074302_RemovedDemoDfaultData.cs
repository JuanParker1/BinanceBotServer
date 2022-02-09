using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class RemovedDemoDfaultData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_settings",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "t_users",
                keyColumn: "id",
                keyValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_users",
                columns: new[] { "id", "date_created", "email", "id_role", "login", "name", "password", "surname" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "dev", "Developer", "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91", null });

            migrationBuilder.InsertData(
                table: "t_settings",
                columns: new[] { "id", "api_key", "id_user", "is_trade_enabled", "limit_order_rate", "secret_key", "trade_mode" },
                values: new object[] { 1, null, 1, false, 25, null, 0 });
        }
    }
}
