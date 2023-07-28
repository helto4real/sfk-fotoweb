using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FotoApi.Infrastructure.Repositories.PhotoServiceDbContext.Migrations
{
    /// <inheritdoc />
    public partial class members : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7af31847-0e6b-4b2b-9bba-11b02116a2b0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf974b14-bcc0-4777-a8a2-60e9bbf19394");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ffa8da21-8554-4f4b-b7f9-a9808307a4e6");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f8b30dba-a463-4f1e-8949-d9118215cad1", "c29b5910-4788-4415-83d7-1a5eb7df498b" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8b30dba-a463-4f1e-8949-d9118215cad1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c29b5910-4788-4415-83d7-1a5eb7df498b");

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerReference = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    AboutMe = table.Column<string>(type: "text", nullable: true),
                    FeePayDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7af31847-0e6b-4b2b-9bba-11b02116a2b0", "62ec3420-93ae-496a-b434-11fbd18e27a8", "Member", "MEMBER" },
                    { "cf974b14-bcc0-4777-a8a2-60e9bbf19394", "9df15112-34ed-4ab7-b864-ef84ae3ba2df", "CompetitionAdministrator", "COMPETITIONADMINISTRATOR" },
                    { "f8b30dba-a463-4f1e-8949-d9118215cad1", "f8b30dba-a463-4f1e-8949-d9118215cad1", "Admin", "ADMIN" },
                    { "ffa8da21-8554-4f4b-b7f9-a9808307a4e6", "10972b9e-8743-433e-ad4d-31814fabe54b", "StbildAdministrator", "STBILDADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpirationDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c29b5910-4788-4415-83d7-1a5eb7df498b", 0, "86b50e0b-631b-431a-9626-db1f1df3ed30", "admin@localhost", false, false, null, "ADMIN@LOCALHOST", "ADMIN", "AQAAAAIAAYagAAAAEPT0XTzpfkbBLcRGYpA/OtTded0h+KjrLMGpmvqAK0AEaW7vpkOaieEkQc95IqZPgw==", null, false, "Some non existing token", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "7b4098d5-93cf-45b0-9d30-569853561aac", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "f8b30dba-a463-4f1e-8949-d9118215cad1", "c29b5910-4788-4415-83d7-1a5eb7df498b" });
        }
    }
}
