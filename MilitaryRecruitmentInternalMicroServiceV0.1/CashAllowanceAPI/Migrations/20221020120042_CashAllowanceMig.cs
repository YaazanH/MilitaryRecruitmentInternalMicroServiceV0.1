using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CashAllowanceAPI.Migrations
{
    public partial class CashAllowanceMig : Migration
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
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
           
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAllowanceDb", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashAllowanceDb");
        }
    }
}
