using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelApprovalAPI.Migrations
{
    public partial class TravelApprovalMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelApprovalDb",
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
                    table.PrimaryKey("PK_TravelApprovalDb", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelApprovalDb");
        }
    }
}
