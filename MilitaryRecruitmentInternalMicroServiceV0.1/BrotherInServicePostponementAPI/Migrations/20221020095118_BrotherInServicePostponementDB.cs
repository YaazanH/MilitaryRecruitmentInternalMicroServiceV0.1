using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrotherInServicePostponementAPI.Migrations
{
    public partial class BrotherInServicePostponementDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrotherInServicePostponementDBS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    DateOfGiven = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateOfEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrotherInServicePostponementDBS", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrotherInServicePostponementDBS");
        }
    }
}
