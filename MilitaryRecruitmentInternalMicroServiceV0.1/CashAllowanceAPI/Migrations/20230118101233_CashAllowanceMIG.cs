using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CashAllowanceAPI.Migrations
{
    public partial class CashAllowanceMIG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashAllowanceDb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAllowanceDb", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RabbitMQobjDBS",
                columns: table => new
                {
                    RequestStatuseID = table.Column<int>(type: "int", nullable: false),
                    ProcID = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JWT = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RabbitMQResponceDBS",
                columns: table => new
                {
                    RequestStatuseID = table.Column<int>(type: "int", nullable: false),
                    ProcID = table.Column<int>(type: "int", nullable: false),
                    Responce = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RequestStatuesDBS",
                columns: table => new
                {
                    ReqStatuesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    PostponmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfRecive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfDone = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuesDBS", x => x.ReqStatuesID);
                });

            migrationBuilder.CreateTable(
                name: "UserInfoDBS",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    JWT = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "AsyncAgeDb",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Statues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncAgeDb", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncAgeDb_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncPaymentDBS",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payed = table.Column<bool>(type: "bit", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    EcashURl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Statues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncPaymentDBS", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK_AsyncPaymentDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsyncAgeDb_RequestStatuesIDReqStatuesID",
                table: "AsyncAgeDb",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncPaymentDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncPaymentDBS",
                column: "RequestStatuesIDReqStatuesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsyncAgeDb");

            migrationBuilder.DropTable(
                name: "AsyncPaymentDBS");

            migrationBuilder.DropTable(
                name: "CashAllowanceDb");

            migrationBuilder.DropTable(
                name: "RabbitMQobjDBS");

            migrationBuilder.DropTable(
                name: "RabbitMQResponceDBS");

            migrationBuilder.DropTable(
                name: "UserInfoDBS");

            migrationBuilder.DropTable(
                name: "RequestStatuesDBS");
        }
    }
}
