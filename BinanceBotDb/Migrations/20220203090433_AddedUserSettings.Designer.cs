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
    [Migration("20220203090433_AddedUserSettings")]
    partial class AddedUserSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

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

                    b.Property<string>("PasswordHash")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password_hash")
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

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IdRole = 1,
                            Login = "dev",
                            Name = "Developer",
                            PasswordHash = "hs9qw7bf864323e5c894a9d031891ddbf8532a5b9eaf3efe7a1561403e6a6f1b3e680b7c37467e6cbfdce29ed6e9640842093"
                        });
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

            modelBuilder.Entity("BinanceBotDb.Models.UserSettings", b =>
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
                        .HasComment("Amount of % from highest price to place limit order");

                    b.Property<string>("SecretKey")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("secret_key")
                        .HasComment("secret key");

                    b.Property<int>("TradeMode")
                        .HasColumnType("integer")
                        .HasColumnName("trade_mode")
                        .HasComment("0 - trade only by signals \n 1 - purchase by signals, sale by limit order");

                    b.HasKey("Id");

                    b.HasIndex("IdUser")
                        .IsUnique();

                    b.ToTable("t_users_settings");

                    b
                        .HasComment("Users trade settings");
                });

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
                    b.HasOne("BinanceBotDb.Models.UserRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("IdRole");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BinanceBotDb.Models.UserSettings", b =>
                {
                    b.HasOne("BinanceBotDb.Models.User", "User")
                        .WithOne("Settings")
                        .HasForeignKey("BinanceBotDb.Models.UserSettings", "IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BinanceBotDb.Models.User", b =>
                {
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
