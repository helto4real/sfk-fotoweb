using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FotoApi.Migrations
{
    /// <inheritdoc />
    public partial class rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "AboutThePhotograper",
                table: "StBilder",
                newName: "AboutThePhotographer");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName", "SortOrder" },
                values: new object[,]
                {
                    { "06e7b21a-2814-472a-8668-d3ebb3fe59f5", "3363d096-fc58-4fc2-a793-a84223515408", "Member", "MEMBER", 0L },
                    { "40200a2d-df6c-4471-9cb5-d912c764a59b", "6caffd88-8cfe-431f-88da-d05c07f6a2d7", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR", 0L },
                    { "9b173cf9-6125-4807-b060-2ba52bfa3da1", "6f6b7257-c11e-4dc4-8fd9-ce1e618dc443", "StbildAdministrator", "STBILDADMINISTRATOR", 0L },
                    { "ef806348-68da-4dd3-a4f6-c776d8b9c224", "ef806348-68da-4dd3-a4f6-c776d8b9c224", "Admin", "ADMIN", 0L }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "a2cdf038-ba23-4384-a85c-34c73b0fb2ff", 0, "dded32b6-09ab-4eb0-b88e-fd3c7c07a990", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAEEs5KArDWxw/7izvsffZ2moGwO4jXBIk2Mxuw5Mf2vKuXX7/8ighin53/8Q+cJV4hQ==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "613d06df-8122-4fc5-a7b5-8ab8bb2e8899", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "ef806348-68da-4dd3-a4f6-c776d8b9c224", "a2cdf038-ba23-4384-a85c-34c73b0fb2ff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06e7b21a-2814-472a-8668-d3ebb3fe59f5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "40200a2d-df6c-4471-9cb5-d912c764a59b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9b173cf9-6125-4807-b060-2ba52bfa3da1");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ef806348-68da-4dd3-a4f6-c776d8b9c224", "a2cdf038-ba23-4384-a85c-34c73b0fb2ff" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef806348-68da-4dd3-a4f6-c776d8b9c224");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a2cdf038-ba23-4384-a85c-34c73b0fb2ff");

            migrationBuilder.RenameColumn(
                name: "AboutThePhotographer",
                table: "StBilder",
                newName: "AboutThePhotograper");

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
    }
}
