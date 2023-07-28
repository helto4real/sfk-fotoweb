using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FotoApi.Infrastructure.Repositories.PhotoServiceDbContext.Migrations
{
    /// <inheritdoc />
    public partial class memberstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21fc145a-752e-4aea-997c-6d5ca1cd4cba");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "225cc625-92b6-47db-b212-7d72a8445d56");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "67b6c5ba-f5b5-4765-a106-63fc4079c232");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "77939015-7a95-4830-bee9-830a13b3ac59", "18302a8f-7230-4d3e-b6e1-91791ebf5b3c" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "77939015-7a95-4830-bee9-830a13b3ac59");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "18302a8f-7230-4d3e-b6e1-91791ebf5b3c");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Members",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b640a0b-eb1f-48d4-adfe-10c660f305be", "ba4ea268-f522-4d9c-888d-a98c8c10bb86", "Member", "MEMBER" },
                    { "7703c71e-26db-4ebf-9f73-2621f4619ea2", "8428c550-1d76-4058-ae20-4a76879f1d5c", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR" },
                    { "9923291f-072a-47d0-bf44-d3005026f33a", "d7014141-e6f1-4cad-9b7e-70c7d75335f7", "StbildAdministrator", "STBILDADMINISTRATOR" },
                    { "f3d3d613-5201-470a-8162-b33ef335247b", "f3d3d613-5201-470a-8162-b33ef335247b", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "f43b04b6-52be-4df9-8538-72f34c5d6938", 0, "4b2f621f-16aa-442e-a5cd-1afe36048bb1", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAEL9ktjaN3cPf3WYsUA0GVlbias/9cw7hqEjWHWq12dGCOCbImNXGlDHWszvd8tmb9A==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "433d6ba7-91bf-4d3f-8cc4-97408ea72bac", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "f3d3d613-5201-470a-8162-b33ef335247b", "f43b04b6-52be-4df9-8538-72f34c5d6938" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b640a0b-eb1f-48d4-adfe-10c660f305be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7703c71e-26db-4ebf-9f73-2621f4619ea2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9923291f-072a-47d0-bf44-d3005026f33a");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f3d3d613-5201-470a-8162-b33ef335247b", "f43b04b6-52be-4df9-8538-72f34c5d6938" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3d3d613-5201-470a-8162-b33ef335247b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f43b04b6-52be-4df9-8538-72f34c5d6938");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Members");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21fc145a-752e-4aea-997c-6d5ca1cd4cba", "fa593cd2-f80a-4ebf-a8a9-3661e670a5f4", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR" },
                    { "225cc625-92b6-47db-b212-7d72a8445d56", "4c56a907-0d78-4db0-9e0b-5f3151392636", "Member", "MEMBER" },
                    { "67b6c5ba-f5b5-4765-a106-63fc4079c232", "ee37dde9-9a9f-4247-8f04-8f3078544c3a", "StbildAdministrator", "STBILDADMINISTRATOR" },
                    { "77939015-7a95-4830-bee9-830a13b3ac59", "77939015-7a95-4830-bee9-830a13b3ac59", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "18302a8f-7230-4d3e-b6e1-91791ebf5b3c", 0, "811954b8-d63c-4867-912b-625b76d5f5bb", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAEPrIWiFLAYbCeug80Pko3PVLMicIFoUbNtApG1TX5r9la7nbnZ9wBMBLNRMsMhNXcw==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "3070cdaa-c202-46aa-9ad1-b5def01874a7", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "77939015-7a95-4830-bee9-830a13b3ac59", "18302a8f-7230-4d3e-b6e1-91791ebf5b3c" });
        }
    }
}
