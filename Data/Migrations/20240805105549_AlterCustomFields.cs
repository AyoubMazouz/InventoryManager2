using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FieldValue",
                table: "CustomField",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "FieldName",
                table: "CustomField",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CustomField",
                newName: "FieldValue");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CustomField",
                newName: "FieldName");
        }
    }
}
