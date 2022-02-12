﻿// <auto-generated />
using System;
using BinanceBotDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    [DbContext(typeof(BinanceBotDbContext))]
    [Migration("20220212130548_AddedRequestLog")]
    partial class AddedRequestLog
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BinanceBotDb.Models.BalanceChange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("double precision")
                        .HasColumnName("amount")
                        .HasComment("Amount of change in USDT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date")
                        .HasComment("Balance change date");

                    b.Property<int>("IdDirection")
                        .HasColumnType("integer")
                        .HasColumnName("id_direction")
                        .HasComment("1 - Deposit \n2 - Withdraw");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("t_balance_changes");

                    b
                        .HasComment("User balance deposits and withdrawals");
                });

            modelBuilder.Entity("BinanceBotDb.Models.Directories.TradeMode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Caption")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("caption")
                        .HasComment("Caption");

                    b.HasKey("Id");

                    b.ToTable("t_trade_modes");

                    b
                        .HasComment("Trade modes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Caption = "Auto"
                        },
                        new
                        {
                            Id = 2,
                            Caption = "Semiauto"
                        });
                });

            modelBuilder.Entity("BinanceBotDb.Models.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date");

                    b.Property<long>("ElapsedMilliseconds")
                        .HasColumnType("bigint")
                        .HasColumnName("elapsed_ms");

                    b.Property<string>("ExceptionMessage")
                        .HasColumnType("text")
                        .HasColumnName("ex_message");

                    b.Property<string>("ExceptionStack")
                        .HasColumnType("text")
                        .HasColumnName("ex_stack");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.Property<string>("Ip")
                        .HasColumnType("text")
                        .HasColumnName("ip");

                    b.Property<string>("Login")
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.Property<string>("Referer")
                        .HasColumnType("text")
                        .HasColumnName("referer");

                    b.Property<string>("RequestMethod")
                        .HasColumnType("text")
                        .HasColumnName("request_method");

                    b.Property<string>("RequestPath")
                        .HasColumnType("text")
                        .HasColumnName("request_path");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("t_request_log");

                    b
                        .HasComment("Application http requests log");
                });

            modelBuilder.Entity("BinanceBotDb.Models.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApiKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("api_key")
                        .HasComment("api key");

                    b.Property<int>("IdTradeMode")
                        .HasColumnType("integer")
                        .HasColumnName("id_trade_mode")
                        .HasComment("Selected trade mode");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.Property<bool>("IsTradeEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("is_trade_enabled")
                        .HasComment("Trade on/off");

                    b.Property<int>("LimitOrderRate")
                        .HasColumnType("integer")
                        .HasColumnName("limit_order_rate")
                        .HasComment("Amount of percents from highest price to place limit order");

                    b.Property<string>("SecretKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secret_key")
                        .HasComment("secret key");

                    b.HasKey("Id");

                    b.HasIndex("IdTradeMode");

                    b.HasIndex("IdUser")
                        .IsUnique();

                    b.ToTable("t_settings");

                    b
                        .HasComment("Application trade settings");
                });

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date_created");

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email")
                        .HasComment("Email");

                    b.Property<int?>("IdRole")
                        .HasColumnType("integer")
                        .HasColumnName("id_role");

                    b.Property<string>("Login")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("login");

                    b.Property<string>("Name")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name")
                        .HasComment("Name");

                    b.Property<string>("Password")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password")
                        .HasComment("Password hash");

                    b.Property<string>("Surname")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("surname")
                        .HasComment("Surname");

                    b.HasKey("Id");

                    b.HasIndex("IdRole");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("t_users");

                    b
                        .HasComment("Users");
                });

            modelBuilder.Entity("BinanceBotDb.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Caption")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("caption")
                        .HasComment("Caption");

                    b.HasKey("Id");

                    b.ToTable("t_user_roles");

                    b
                        .HasComment("User roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Caption = "Administrator"
                        },
                        new
                        {
                            Id = 2,
                            Caption = "User"
                        });
                });

            modelBuilder.Entity("BinanceBotDb.Models.BalanceChange", b =>
                {
                    b.HasOne("BinanceBotDb.Models.User", "User")
                        .WithMany("BalanceChanges")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BinanceBotDb.Models.Request", b =>
                {
                    b.HasOne("BinanceBotDb.Models.User", "User")
                        .WithMany("RequestLog")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BinanceBotDb.Models.Settings", b =>
                {
                    b.HasOne("BinanceBotDb.Models.Directories.TradeMode", "TradeMode")
                        .WithMany("Settings")
                        .HasForeignKey("IdTradeMode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BinanceBotDb.Models.User", "User")
                        .WithOne("Settings")
                        .HasForeignKey("BinanceBotDb.Models.Settings", "IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TradeMode");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
                    b.HasOne("BinanceBotDb.Models.UserRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("IdRole");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BinanceBotDb.Models.Directories.TradeMode", b =>
                {
                    b.Navigation("Settings");
                });

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
                    b.Navigation("BalanceChanges");

                    b.Navigation("RequestLog");

                    b.Navigation("Settings");
                });

            modelBuilder.Entity("BinanceBotDb.Models.UserRole", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
