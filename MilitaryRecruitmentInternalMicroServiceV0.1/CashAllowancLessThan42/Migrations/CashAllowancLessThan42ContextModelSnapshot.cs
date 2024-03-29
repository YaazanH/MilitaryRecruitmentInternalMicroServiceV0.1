﻿// <auto-generated />
using System;
using CashAllowancLessThan42.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CashAllowancLessThan42.Migrations
{
    [DbContext(typeof(CashAllowancLessThan42Context))]
    partial class CashAllowancLessThan42ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncAge", b =>
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

                    b.Property<string>("statuse")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsyncAgeDBS");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncDaysOutsideCoun", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DaysOutsideCoun")
                        .HasColumnType("int");

                    b.Property<DateTime>("RequestReciveTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequestSendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RequestStatuesIDReqStatuesID")
                        .HasColumnType("int");

                    b.Property<string>("statuse")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsyncDaysOutsideCounDBS");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncPayment", b =>
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

            modelBuilder.Entity("CashAllowancLessThan42.Models.Asynctravel", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("RequestReciveTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RequestSendTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RequestStatuesIDReqStatuesID")
                        .HasColumnType("int");

                    b.Property<string>("statuse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("travel")
                        .HasColumnType("bit");

                    b.HasKey("ID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsynctravelDBS");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.CashAllowancLessThan42Model", b =>
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

                    b.ToTable("CashAllowancLessThan42Db");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.RabbitMQResponce", b =>
                {
                    b.Property<int>("ProcID")
                        .HasColumnType("int");

                    b.Property<int>("RequestStatuseID")
                        .HasColumnType("int");

                    b.Property<string>("Responce")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RabbitMQResponceDBS");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.RabbitMQobj", b =>
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

            modelBuilder.Entity("CashAllowancLessThan42.Models.RequestStatues", b =>
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

            modelBuilder.Entity("CashAllowancLessThan42.Models.UserInfo", b =>
                {
                    b.Property<string>("JWT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.ToTable("UserInfoDBS");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncAge", b =>
                {
                    b.HasOne("CashAllowancLessThan42.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncDaysOutsideCoun", b =>
                {
                    b.HasOne("CashAllowancLessThan42.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.AsyncPayment", b =>
                {
                    b.HasOne("CashAllowancLessThan42.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("CashAllowancLessThan42.Models.Asynctravel", b =>
                {
                    b.HasOne("CashAllowancLessThan42.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });
#pragma warning restore 612, 618
        }
    }
}
