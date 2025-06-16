using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddConsumptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumption_Meters_MeterId",
                table: "Consumption");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Consumption_ConsumptionId",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consumption",
                table: "Consumption");

            migrationBuilder.RenameTable(
                name: "Consumption",
                newName: "Consumptions");

            migrationBuilder.RenameIndex(
                name: "IX_Consumption_MeterId",
                table: "Consumptions",
                newName: "IX_Consumptions_MeterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consumptions",
                table: "Consumptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumptions_Meters_MeterId",
                table: "Consumptions",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Consumptions_ConsumptionId",
                table: "Invoice",
                column: "ConsumptionId",
                principalTable: "Consumptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumptions_Meters_MeterId",
                table: "Consumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Consumptions_ConsumptionId",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Consumptions",
                table: "Consumptions");

            migrationBuilder.RenameTable(
                name: "Consumptions",
                newName: "Consumption");

            migrationBuilder.RenameIndex(
                name: "IX_Consumptions_MeterId",
                table: "Consumption",
                newName: "IX_Consumption_MeterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Consumption",
                table: "Consumption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumption_Meters_MeterId",
                table: "Consumption",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Consumption_ConsumptionId",
                table: "Invoice",
                column: "ConsumptionId",
                principalTable: "Consumption",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
