﻿// <auto-generated />
using System;
using CashAllowanceAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashAllowanceAPI.Migrations
{
    [DbContext(typeof(CashAllowanceContext))]
    [Migration("20230118101233_CashAllowanceMIG")]
    partial class CashAllowanceMIG
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CashAllowanceAPI.Models.AsyncAge", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<DateTime>("RequestReciveTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequestSendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RequestStatuesIDReqStatuesID")
                        .HasColumnType("int");

                    b.Property<string>("Statues")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsyncAgeDb");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.AsyncPayment", b =>
                {
                    b.Property<int>("PaymentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<string>("EcashURl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Payed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RequestReciveTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequestSendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RequestStatuesIDReqStatuesID")
                        .HasColumnType("int");

                    b.Property<string>("Statues")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsyncPaymentDBS");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.CashAllowance", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("DateOfGiven")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("CashAllowanceDb");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.RabbitMQResponce", b =>
                {
                    b.Property<int>("ProcID")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatuseID")
                        .HasColumnType("int");

                    b.Property<string>("Responce")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RabbitMQResponceDBS");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.RabbitMQobj", b =>
                {
                    b.Property<string>("JWT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProcID")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatuseID")
                        .HasColumnType("int");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.ToTable("RabbitMQobjDBS");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.RequestStatues", b =>
                {
                    b.Property<int>("ReqStatuesID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateOfDone")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfRecive")
                        .HasColumnType("datetime2");

                    b.Property<string>("PostponmentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Statues")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ReqStatuesID");

                    b.ToTable("RequestStatuesDBS");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.UserInfo", b =>
                {
                    b.Property<string>("JWT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.ToTable("UserInfoDBS");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.AsyncAge", b =>
                {
                    b.HasOne("CashAllowanceAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("CashAllowanceAPI.Models.AsyncPayment", b =>
                {
                    b.HasOne("CashAllowanceAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });
#pragma warning restore 612, 618
        }
    }
}