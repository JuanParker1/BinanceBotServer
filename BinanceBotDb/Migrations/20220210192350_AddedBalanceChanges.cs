using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedBalanceChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_balance_changes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Balance change date"),
                    id_direction = table.Column<int>(type: "integer", nullable: false, comment: "1 - Deposit \n2 - Withdraw"),
                    amount = table.Column<int>(type: "integer", nullable: false, comment: "Amount of change in USDT")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_balance_changes", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_balance_changes_t_users_id_user",
                        column: x => x.id_user,
                        principalTable: "t_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "User balance deposits and withdrawals");

            migrationBuilder.CreateIndex(
                name: "IX_t_balance_changes_id_user",
                table: "t_balance_changes",
                column: "id_user");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_balance_changes");
        }
    }
}
