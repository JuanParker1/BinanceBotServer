using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class AddedSettingsDefaultData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_settings",
                columns: new[] { "id", "api_key", "id_user", "is_trade_enabled", "limit_order_rate", "secret_key", "trade_mode" },
                values: new object[] { 1, null, 1, false, 25, null, 0 });

            migrationBuilder.UpdateData(
                table: "t_users",
                keyColumn: "id",
                keyValue: 1,
                column: "password",
                value: "VzwA|6a4e3df1193666839c57ac8dcafe549cfb00fab0fdd78a008261332ba5c1a326ab93b6993a913219c2f8e078103b8f91");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_settings",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "t_users",
                keyColumn: "id",
                keyValue: 1,
                column: "password",
                value: "hs9qw7bf864323e5c894a9d031891ddbf8532a5b9eaf3efe7a1561403e6a6f1b3e680b7c37467e6cbfdce29ed6e9640842093");
        }
    }
}
