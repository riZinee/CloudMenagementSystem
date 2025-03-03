using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "HomeCatalog",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "StorageMetadata",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "StorageMetadata",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StorageMetadata_ParentId",
                table: "StorageMetadata",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StorageMetadata_StorageMetadata_ParentId",
                table: "StorageMetadata",
                column: "ParentId",
                principalTable: "StorageMetadata",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorageMetadata_StorageMetadata_ParentId",
                table: "StorageMetadata");

            migrationBuilder.DropIndex(
                name: "IX_StorageMetadata_ParentId",
                table: "StorageMetadata");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "StorageMetadata");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "StorageMetadata");

            migrationBuilder.AlterColumn<string>(
                name: "HomeCatalog",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
