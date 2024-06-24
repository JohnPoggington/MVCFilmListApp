using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCFilmLists.Migrations
{
    /// <inheritdoc />
    public partial class SimplestManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListEntries");

            migrationBuilder.CreateTable(
                name: "MovieMovieList",
                columns: table => new
                {
                    MoviesId = table.Column<int>(type: "int", nullable: false),
                    movieListsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieMovieList", x => new { x.MoviesId, x.movieListsId });
                    table.ForeignKey(
                        name: "FK_MovieMovieList_MovieLists_movieListsId",
                        column: x => x.movieListsId,
                        principalTable: "MovieLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieMovieList_Movie_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieMovieList_movieListsId",
                table: "MovieMovieList",
                column: "movieListsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieMovieList");

            migrationBuilder.CreateTable(
                name: "ListEntries",
                columns: table => new
                {
                    MovieListId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListEntries", x => new { x.MovieListId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_ListEntries_MovieLists_MovieListId",
                        column: x => x.MovieListId,
                        principalTable: "MovieLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListEntries_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListEntries_MovieId",
                table: "ListEntries",
                column: "MovieId");
        }
    }
}
