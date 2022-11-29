using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlonePostponement.Migrations
{
    public partial class AlonePostmentMIGR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlonePostponementDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateOfEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlonePostponementDBS", x => x.ID);
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
                name: "BrotherEillDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllBrotherEill = table.Column<bool>(type: "bit", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrotherEillDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BrotherEillDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BrothersIDDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrotherID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrothersIDDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BrothersIDDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeadBrothersDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AllDeadBrothers = table.Column<bool>(type: "bit", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeadBrothersDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeadBrothersDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HaveBrothersDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestStatuesIDReqStatuesID = table.Column<int>(type: "int", nullable: true),
                    RequestSendTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HaveBrother = table.Column<bool>(type: "bit", nullable: false),
                    RequestReciveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HaveBrothersDBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HaveBrothersDBS_RequestStatuesDBS_RequestStatuesIDReqStatuesID",
                        column: x => x.RequestStatuesIDReqStatuesID,
                        principalTable: "RequestStatuesDBS",
                        principalColumn: "ReqStatuesID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrotherEillDBS_RequestStatuesIDReqStatuesID",
                table: "BrotherEillDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_BrothersIDDBS_RequestStatuesIDReqStatuesID",
                table: "BrothersIDDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_DeadBrothersDBS_RequestStatuesIDReqStatuesID",
                table: "DeadBrothersDBS",
                column: "RequestStatuesIDReqStatuesID");

            migrationBuilder.CreateIndex(
                name: "IX_HaveBrothersDBS_RequestStatuesIDReqStatuesID",
                table: "HaveBrothersDBS",
                column: "RequestStatuesIDReqStatuesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlonePostponementDBS");

            migrationBuilder.DropTable(
                name: "BrotherEillDBS");

            migrationBuilder.DropTable(
                name: "BrothersIDDBS");

            migrationBuilder.DropTable(
                name: "DeadBrothersDBS");

            migrationBuilder.DropTable(
                name: "HaveBrothersDBS");

            migrationBuilder.DropTable(
                name: "RequestStatuesDBS");
        }
    }
}
