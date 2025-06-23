using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Consumptions_ConsumptionId",
                table: "Invoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.RenameIndex(
                name: "IX_Invoice_ConsumptionId",
                table: "Invoices",
                newName: "IX_Invoices_ConsumptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Consumptions_ConsumptionId",
                table: "Invoices",
                column: "ConsumptionId",
                principalTable: "Consumptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Consumptions_ConsumptionId",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_ConsumptionId",
                table: "Invoice",
                newName: "IX_Invoice_ConsumptionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Consumptions_ConsumptionId",
                table: "Invoice",
                column: "ConsumptionId",
                principalTable: "Consumptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
