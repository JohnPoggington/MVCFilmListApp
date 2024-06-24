using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCFilmLists.Migrations
{
    /// <inheritdoc />
    public partial class ListEntryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListEntries",
                table: "ListEntries");

            migrationBuilder.DropIndex(
                name: "IX_ListEntries_MovieListId",
                table: "ListEntries");

            migrationBuilder.DropColumn(
                name: "ListId",
                table: "ListEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListEntries",
                table: "ListEntries",
                columns: new[] { "MovieListId", "MovieId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListEntries",
                table: "ListEntries");

            migrationBuilder.AddColumn<int>(
                name: "ListId",
                table: "ListEntries",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListEntries",
                table: "ListEntries",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_ListEntries_MovieListId",
                table: "ListEntries",
                column: "MovieListId");
        }
    }
}
