using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FotoApi.Migrations
{
    /// <inheritdoc />
    public partial class sortorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "72ebb247-7c9a-439a-8290-c3cfc093f87b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9128425e-7971-4c09-a685-1f53eb4a32ff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d81d8bf5-d073-4f12-948d-a90de022c1d6");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "73a8f14f-6208-4fa9-bada-4d1a2ab84f42", "30b56bef-ee2a-40ae-a776-2ee6db5e0002" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73a8f14f-6208-4fa9-bada-4d1a2ab84f42");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "30b56bef-ee2a-40ae-a776-2ee6db5e0002");

            migrationBuilder.AddColumn<long>(
                name: "SortOrder",
                table: "AspNetRoles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName", "SortOrder" },
                values: new object[,]
                {
                    { "006c4fab-710f-4e5a-8dcf-663686ef8ac5", "8c8b848e-54cd-40a5-9a70-35c2d9f4fd8f", "Member", "MEMBER", 0L },
                    { "00da0dc7-4940-432d-830c-6277445d2104", "4f511b14-296e-4e37-8c8a-bb0a7f89ec04", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR", 0L },
                    { "d91f9a40-b4be-4f6d-af5b-022f7e47e3e7", "d91f9a40-b4be-4f6d-af5b-022f7e47e3e7", "Admin", "ADMIN", 0L },
                    { "efb4cb90-33a9-4899-bbb8-b1d01eace7b9", "5427b67e-8380-4a7f-b421-c4ca91f73757", "StbildAdministrator", "STBILDADMINISTRATOR", 0L }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4d12b994-5369-4652-bd5e-f99ae7def087", 0, "ad53e43f-cdde-4e47-a053-968ba3925f2d", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAECHynwwecR/7f/Yf2wcN0quGCPMOIzVShc3x0DsG5k/vHi67ikTczSrrxomFmGi7Ug==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "aaec8f23-5bf8-449e-9694-d9f9077604b7", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "d91f9a40-b4be-4f6d-af5b-022f7e47e3e7", "4d12b994-5369-4652-bd5e-f99ae7def087" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "006c4fab-710f-4e5a-8dcf-663686ef8ac5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "00da0dc7-4940-432d-830c-6277445d2104");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "efb4cb90-33a9-4899-bbb8-b1d01eace7b9");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "d91f9a40-b4be-4f6d-af5b-022f7e47e3e7", "4d12b994-5369-4652-bd5e-f99ae7def087" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d91f9a40-b4be-4f6d-af5b-022f7e47e3e7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4d12b994-5369-4652-bd5e-f99ae7def087");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AspNetRoles");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "72ebb247-7c9a-439a-8290-c3cfc093f87b", "14001131-1daa-4921-9613-e9aefd971d32", "StbildAdministrator", "STBILDADMINISTRATOR" },
                    { "73a8f14f-6208-4fa9-bada-4d1a2ab84f42", "73a8f14f-6208-4fa9-bada-4d1a2ab84f42", "Admin", "ADMIN" },
                    { "9128425e-7971-4c09-a685-1f53eb4a32ff", "455c8ffc-d8e6-473e-be16-a770e5324dcb", "Member", "MEMBER" },
                    { "d81d8bf5-d073-4f12-948d-a90de022c1d6", "4f4a6cb0-c4d9-4295-acf8-9784b53be663", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "30b56bef-ee2a-40ae-a776-2ee6db5e0002", 0, "26ed1d81-c79b-4cd8-b5ab-858bc35bc32a", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAEDpkZNqbZ1oO1n5f6gCiqoKT5tP2kQn+x9x7lJRDogChq+Iz5qo67aVszRLbDjpz8w==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "551dc29d-e5a9-4cfb-b03f-16eb6af039d2", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "73a8f14f-6208-4fa9-bada-4d1a2ab84f42", "30b56bef-ee2a-40ae-a776-2ee6db5e0002" });
        }
    }
}
