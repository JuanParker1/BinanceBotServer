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
    [Migration("20220217103745_RemovedErrorRequestsLogging")]
    partial class RemovedErrorRequestsLogging
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

            modelBuilder.Entity("BinanceBotDb.Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("date")
                        .HasComment("Дата совершения события");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean")
                        .HasColumnName("is_read")
                        .HasComment("Shows if event was read by user or not");

                    b.Property<string>("Text")
                        .HasMaxLength(700)
                        .HasColumnType("character varying(700)")
                        .HasColumnName("text")
                        .HasComment("Event text");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("t_events");

                    b
                        .HasComment("User/application event log");
                });

            modelBuilder.Entity("BinanceBotDb.Models.EventTemplate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Template")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("template")
                        .HasComment("Template text");

                    b.HasKey("Id");

                    b.ToTable("t_event_templates");

                    b
                        .HasComment("Event log text templates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Template = "Совершена покупка торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}."
                        },
                        new
                        {
                            Id = 2,
                            Template = "Совершена продажа торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}."
                        },
                        new
                        {
                            Id = 3,
                            Template = "На бирже установлен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}."
                        },
                        new
                        {
                            Id = 4,
                            Template = "На бирже отменен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}."
                        },
                        new
                        {
                            Id = 5,
                            Template = "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}."
                        },
                        new
                        {
                            Id = 6,
                            Template = "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}."
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
                        .HasColumnName("date")
                        .HasComment("Request date");

                    b.Property<long>("ElapsedMilliseconds")
                        .HasColumnType("bigint")
                        .HasColumnName("elapsed_ms")
                        .HasComment("Request lifetime");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer")
                        .HasColumnName("id_user")
                        .HasComment("Request user id");

                    b.Property<string>("Ip")
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)")
                        .HasColumnName("ip")
                        .HasComment("Request ip address");

                    b.Property<string>("Login")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("login")
                        .HasComment("Request user login");

                    b.Property<string>("Referer")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("referer")
                        .HasComment("Request referer");

                    b.Property<string>("RequestMethod")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("request_method")
                        .HasComment("Request http method");

                    b.Property<string>("RequestPath")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("request_path")
                        .HasComment("Request path");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status")
                        .HasComment("Request http status");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("t_request_log");

                    b
                        .HasComment("Application http request log");
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

            modelBuilder.Entity("BinanceBotDb.Models.Event", b =>
                {
                    b.HasOne("BinanceBotDb.Models.User", "User")
                        .WithMany("EventLog")
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

                    b.Navigation("EventLog");

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
