using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lumo.Data.Migrations
{
    /// <inheritdoc />
    public partial class Tags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResourceKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsGlobal = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "CustomName", "IsGlobal", "ResourceKey", "UserId" },
                values: new object[,]
                {
                    { 1, null, true, "Tag.Work", null },
                    { 2, null, true, "Tag.Family", null },
                    { 3, null, true, "Tag.Health", null }
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
