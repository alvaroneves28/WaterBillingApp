using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterBillingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndCustomerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumption_Meter_MeterId",
                table: "Consumption");

            migrationBuilder.DropForeignKey(
                name: "FK_Meter_Customers_CustomerId",
                table: "Meter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meter",
                table: "Meter");

            migrationBuilder.RenameTable(
                name: "Meter",
                newName: "Meters");

            migrationBuilder.RenameIndex(
                name: "IX_Meter_CustomerId",
                table: "Meters",
                newName: "IX_Meters_CustomerId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NIF",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meters",
                table: "Meters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumption_Meters_MeterId",
                table: "Consumption",
                column: "MeterId",
                principalTable: "Meters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meters_Customers_CustomerId",
                table: "Meters",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consumption_Meters_MeterId",
                table: "Consumption");

            migrationBuilder.DropForeignKey(
                name: "FK_Meters_Customers_CustomerId",
                table: "Meters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meters",
                table: "Meters");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NIF",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Meters",
                newName: "Meter");

            migrationBuilder.RenameIndex(
                name: "IX_Meters_CustomerId",
                table: "Meter",
                newName: "IX_Meter_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meter",
                table: "Meter",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consumption_Meter_MeterId",
                table: "Consumption",
                column: "MeterId",
                principalTable: "Meter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meter_Customers_CustomerId",
                table: "Meter",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
