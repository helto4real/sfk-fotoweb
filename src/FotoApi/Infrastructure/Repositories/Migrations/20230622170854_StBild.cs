using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FotoApi.Infrastructure.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class StBild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraBrand",
                table: "StBilder");

            migrationBuilder.DropColumn(
                name: "FavoriteSubject",
                table: "StBilder");

            migrationBuilder.DropColumn(
                name: "FavouritePhotographer",
                table: "StBilder");

            migrationBuilder.RenameColumn(
                name: "PhotoLocation",
                table: "StBilder",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "HomePage",
                table: "StBilder",
                newName: "AboutThePhotograper");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "StBilder",
                newName: "PhotoLocation");

            migrationBuilder.RenameColumn(
                name: "AboutThePhotograper",
                table: "StBilder",
                newName: "HomePage");

            migrationBuilder.AddColumn<string>(
                name: "CameraBrand",
                table: "StBilder",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FavoriteSubject",
                table: "StBilder",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FavouritePhotographer",
                table: "StBilder",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
