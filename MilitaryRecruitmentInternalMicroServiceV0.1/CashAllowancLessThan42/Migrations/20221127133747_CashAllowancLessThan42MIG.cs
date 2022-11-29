using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CashAllowancLessThan42.Migrations
{
    public partial class CashAllowancLessThan42MIG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashAllowancLessThan42Db",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAllowancLessThan42Db", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatuesDBS",
                columns: table => new
                {
                    ReqStatuesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfRecive = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfDone = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuesDBS", x => x.ReqStatuesID);
                });

            migrationBuilder.CreateTable(
                name: "AsyncAgeDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncAgeDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncAgeDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncDaysOutsideCounDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaysOutsideCoun = table.Column<int>(type: "int", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncDaysOutsideCounDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncDaysOutsideCounDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsynctravelDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    travel = table.Column<bool>(type: "bit", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsynctravelDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsynctravelDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsyncUserTransactionsDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserTransactions = table.Column<bool>(type: "bit", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsyncUserTransactionsDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AsyncUserTransactionsDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsyncAgeDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncAgeDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncDaysOutsideCounDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncDaysOutsideCounDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsynctravelDBS_RequestStatuesIDReqStatuesID",
                table: "AsynctravelDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_AsyncUserTransactionsDBS_RequestStatuesIDReqStatuesID",
                table: "AsyncUserTransactionsDBS",
                column: "RequestStatuesIDReqStatuesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsyncAgeDBS");

            migrationBuilder.DropTable(
                name: "AsyncDaysOutsideCounDBS");

            migrationBuilder.DropTable(
                name: "AsynctravelDBS");

            migrationBuilder.DropTable(
                name: "AsyncUserTransactionsDBS");

            migrationBuilder.DropTable(
                name: "CashAllowancLessThan42Db");

            migrationBuilder.DropTable(
                name: "RequestStatuesDBS");
        }
    }
}
