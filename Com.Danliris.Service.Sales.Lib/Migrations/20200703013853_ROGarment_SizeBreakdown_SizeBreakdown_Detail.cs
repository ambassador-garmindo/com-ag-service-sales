using Microsoft.EntityFrameworkCore.Migrations;

namespace Com.Danliris.Service.Sales.Lib.Migrations
{
    public partial class ROGarment_SizeBreakdown_SizeBreakdown_Detail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeBreakdownIndex",
                table: "RO_Garment_SizeBreakdowns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeBreakdownDetailIndex",
                table: "RO_Garment_SizeBreakdown_Details",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeBreakdownIndex",
                table: "RO_Garment_SizeBreakdowns");

            migrationBuilder.DropColumn(
                name: "SizeBreakdownDetailIndex",
                table: "RO_Garment_SizeBreakdown_Details");
        }
    }
}
