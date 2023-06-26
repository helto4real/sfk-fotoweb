using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FotoApi.Infrastructure.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class StBildAccepted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "StBilder",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "StBilder");
        }
    }
}
