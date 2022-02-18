using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedOrdersModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_order_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Caption = table.Column<string>(type: "text", nullable: true, comment: "Order type caption")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order_types", x => x.id);
                },
                comment: "Order types");

            migrationBuilder.CreateTable(
                name: "t_orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Order creation date"),
                    symbol = table.Column<string>(type: "text", nullable: true, comment: "Trade pair"),
                    id_side = table.Column<int>(type: "integer", nullable: false, comment: "1 - Buy\n2 - Sell"),
                    id_type = table.Column<int>(type: "integer", nullable: false, comment: "Order type id"),
                    id_creation_type = table.Column<int>(type: "integer", nullable: false, comment: "1 - Auto\n2 - Manual"),
                    id_time_in_force = table.Column<int>(type: "integer", nullable: false, comment: "1 - Full\n2 - Partial"),
                    quantity = table.Column<double>(type: "double precision", nullable: false, comment: "Amount of base asset"),
                    quote_order_qty = table.Column<double>(type: "double precision", nullable: false, comment: "Amount of secondary asset"),
                    price = table.Column<double>(type: "double precision", nullable: false, comment: "Order price in secondary asset of trading pair"),
                    coin_price = table.Column<double>(type: "double precision", nullable: false, comment: "Current coin price (when order was created) in secondary asset of trading pair")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_orders_t_order_types_id_type",
                        column: x => x.id_type,
                        principalTable: "t_order_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Trade orders");

            migrationBuilder.InsertData(
                table: "t_order_types",
                columns: new[] { "id", "Caption" },
                values: new object[,]
                {
                    { 1, "LIMIT" },
                    { 2, "MARKET" },
                    { 3, "STOP_LOSS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_orders_id_type",
                table: "t_orders",
                column: "id_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_orders");

            migrationBuilder.DropTable(
                name: "t_order_types");
        }
    }
}
