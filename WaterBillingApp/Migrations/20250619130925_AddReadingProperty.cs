using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReadingProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Reading",
                table: "Consumptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reading",
                table: "Consumptions");

            migrationBuilder.AlterColumn<double>(
                name: "Volume",
                table: "Consumptions",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
