using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedOrderStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_order_status",
                table: "t_orders",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Order status (NEW, CLOSED, FILLED, etc)");

            migrationBuilder.CreateTable(
                name: "t_order_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Caption = table.Column<string>(type: "text", nullable: true, comment: "Order status caption")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order_status", x => x.id);
                },
                comment: "Order status");

            migrationBuilder.InsertData(
                table: "t_order_status",
                columns: new[] { "id", "Caption" },
                values: new object[,]
                {
                    { 1, "NEW" },
                    { 2, "CANCELLED" },
                    { 3, "FILLED" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_orders_id_order_status",
                table: "t_orders",
                column: "id_order_status");

            migrationBuilder.AddForeignKey(
                name: "FK_t_orders_t_order_status_id_order_status",
                table: "t_orders",
                column: "id_order_status",
                principalTable: "t_order_status",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_orders_t_order_status_id_order_status",
                table: "t_orders");

            migrationBuilder.DropTable(
                name: "t_order_status");

            migrationBuilder.DropIndex(
                name: "IX_t_orders_id_order_status",
                table: "t_orders");

            migrationBuilder.DropColumn(
                name: "id_order_status",
                table: "t_orders");
        }
    }
}
