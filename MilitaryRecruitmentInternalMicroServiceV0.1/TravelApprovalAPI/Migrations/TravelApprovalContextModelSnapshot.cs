﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TravelApprovalAPI.Data;

namespace TravelApprovalAPI.Migrations
{
    [DbContext(typeof(TravelApprovalContext))]
    partial class TravelApprovalContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TravelApprovalAPI.Models.AsynLabor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsALaborWorker")
                        .HasColumnType("bit");

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

                    b.ToTable("AsynLaborDBS");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.AsyncAge", b =>
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

            modelBuilder.Entity("TravelApprovalAPI.Models.AsyncUserTransactions", b =>
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

                    b.Property<bool>("UserTransactions")
                        .HasColumnType("bit");

                    b.Property<string>("statuse")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("RequestStatuesIDReqStatuesID");

                    b.ToTable("AsyncUserTransactionsDBS");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.Asynctravel", b =>
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

            modelBuilder.Entity("TravelApprovalAPI.Models.RabbitMQResponce", b =>
                {
                    b.Property<int>("ProcID")
                        .HasColumnType("int");

                    b.Property<string>("Responce")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("RabbitMQResponceDBS");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.RabbitMQobj", b =>
                {
                    b.Property<string>("JWT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProcID")
                        .HasColumnType("int");

                    b.Property<string>("URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.ToTable("RabbitMQobjDBS");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.RequestStatues", b =>
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

            modelBuilder.Entity("TravelApprovalAPI.Models.TravelApproval", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("DateOfEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("DateOfGiven")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("TravelApprovalDb");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.UserInfo", b =>
                {
                    b.Property<string>("JWT")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.ToTable("UserInfoDBS");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.AsynLabor", b =>
                {
                    b.HasOne("TravelApprovalAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.AsyncAge", b =>
                {
                    b.HasOne("TravelApprovalAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.AsyncUserTransactions", b =>
                {
                    b.HasOne("TravelApprovalAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });

            modelBuilder.Entity("TravelApprovalAPI.Models.Asynctravel", b =>
                {
                    b.HasOne("TravelApprovalAPI.Models.RequestStatues", "RequestStatuesID")
                        .WithMany()
                        .HasForeignKey("RequestStatuesIDReqStatuesID");

                    b.Navigation("RequestStatuesID");
                });
#pragma warning restore 612, 618
        }
    }
}
