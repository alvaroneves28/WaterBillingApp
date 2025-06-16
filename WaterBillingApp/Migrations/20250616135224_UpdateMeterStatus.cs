using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMeterStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Meters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Meters");
        }
    }
}
