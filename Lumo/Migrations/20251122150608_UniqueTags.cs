using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lumo.Migrations
{
    /// <inheritdoc />
    public partial class UniqueTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_ResourceKey_UserId_CustomName",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_DiaryEntries_UserId",
                table: "DiaryEntries");

            migrationBuilder.AlterColumn<string>(
                name: "ResourceKey",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId_CustomName",
                table: "Tags",
                columns: new[] { "UserId", "CustomName" },
                unique: true,
                filter: "[UserId] IS NOT NULL AND [CustomName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_UserId_EntryDate",
                table: "DiaryEntries",
                columns: new[] { "UserId", "EntryDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId_CustomName",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_DiaryEntries_UserId_EntryDate",
                table: "DiaryEntries");

            migrationBuilder.AlterColumn<string>(
                name: "ResourceKey",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ResourceKey_UserId_CustomName",
                table: "Tags",
                columns: new[] { "ResourceKey", "UserId", "CustomName" },
                unique: true,
                filter: "[ResourceKey] IS NOT NULL AND [UserId] IS NOT NULL AND [CustomName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_UserId",
                table: "DiaryEntries",
                column: "UserId");
        }
    }
}
