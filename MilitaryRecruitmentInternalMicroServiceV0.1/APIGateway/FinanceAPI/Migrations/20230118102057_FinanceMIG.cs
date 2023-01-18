using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanceAPI.Migrations
{
    public partial class FinanceMIG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinanceContextDBS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceContextDBS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "OrderRefDBS",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PosponemtName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderRefDBS", x => x.OrderID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinanceContextDBS");

            migrationBuilder.DropTable(
                name: "OrderRefDBS");
        }
    }
}
