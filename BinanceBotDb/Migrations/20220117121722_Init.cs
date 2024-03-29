﻿using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    caption = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Caption")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_roles", x => x.id);
                },
                comment: "User roles");

            migrationBuilder.CreateTable(
                name: "t_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_role = table.Column<int>(type: "integer", nullable: true),
                    login = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Password hash"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Name"),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Surname"),
                    patronymic = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Patronymic"),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Email")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_users_t_user_roles_id_role",
                        column: x => x.id_role,
                        principalTable: "t_user_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Users");

            migrationBuilder.InsertData(
                table: "t_user_roles",
                columns: new[] { "id", "caption" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "t_users",
                columns: new[] { "id", "email", "id_role", "login", "name", "password_hash", "patronymic", "surname" },
                values: new object[] { 1, null, 1, "dev", "Developer", "hs9qw7bf864323e5c894a9d031891ddbf8532a5b9eaf3efe7a1561403e6a6f1b3e680b7c37467e6cbfdce29ed6e9640842093", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_t_users_id_role",
                table: "t_users",
                column: "id_role");

            migrationBuilder.CreateIndex(
                name: "IX_t_users_login",
                table: "t_users",
                column: "login",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_users");

            migrationBuilder.DropTable(
                name: "t_user_roles");
        }
    }
}
