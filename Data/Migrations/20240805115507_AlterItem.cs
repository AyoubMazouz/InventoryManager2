using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManager2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomField_Items_ItemId",
                table: "CustomField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomField",
                table: "CustomField");

            migrationBuilder.RenameTable(
                name: "CustomField",
                newName: "CustomFields");

            migrationBuilder.RenameIndex(
                name: "IX_CustomField_ItemId",
                table: "CustomFields",
                newName: "IX_CustomFields_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomFields",
                table: "CustomFields",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomFields_Items_ItemId",
                table: "CustomFields",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomFields_Items_ItemId",
                table: "CustomFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomFields",
                table: "CustomFields");

            migrationBuilder.RenameTable(
                name: "CustomFields",
                newName: "CustomField");

            migrationBuilder.RenameIndex(
                name: "IX_CustomFields_ItemId",
                table: "CustomField",
                newName: "IX_CustomField_ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomField",
                table: "CustomField",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomField_Items_ItemId",
                table: "CustomField",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
