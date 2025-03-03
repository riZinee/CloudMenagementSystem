using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RepairRelations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "StorageMetadata",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
