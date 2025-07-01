using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTariffBrackets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MinVolume",
                table: "TariffBrackets",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxVolume",
                table: "TariffBrackets",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Invoices",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "TariffBracketId",
                table: "Consumptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consumptions_TariffBracketId",
                table: "Consumptions",
                column: "TariffBracketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_TariffBrackets_TariffBracketId",
                table: "Consumptions",
                column: "TariffBracketId",
                principalTable: "TariffBrackets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_TariffBrackets_TariffBracketId",
                table: "Consumptions");

            migrationBuilder.DropIndex(
                name: "IX_Consumptions_TariffBracketId",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "TariffBracketId",
                table: "Consumptions");

            migrationBuilder.AlterColumn<double>(
                name: "MinVolume",
                table: "TariffBrackets",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)",
                oldPrecision: 10,
                oldScale: 3);

            migrationBuilder.AlterColumn<double>(
                name: "MaxVolume",
                table: "TariffBrackets",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,3)",
                oldPrecision: 10,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Invoices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);
        }
    }
}
