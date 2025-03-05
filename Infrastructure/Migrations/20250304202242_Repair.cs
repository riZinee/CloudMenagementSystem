using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Repair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentFolderId", // Stara nazwa
                table: "StorageMetadata", // Nazwa tabeli
                newName: "ParentDirectoryId" // Nowa nazwa
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentDirectoryId", // Cofnięcie zmiany
                table: "StorageMetadata",
                newName: "ParentFolderId"
            );
        }
    }
}
