using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VacationManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolesAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4d1343e8-06af-4a41-9d44-cbda3a770602");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "51fa6aa9-274c-4cb7-9354-bcce1943b4a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97e3eb47-7185-4b9a-b849-7eb9a1b66cc3");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "admin-role-id", null, "Admin", "ADMIN" },
                    { "developer-role-id", null, "Developer", "DEVELOPER" },
                    { "teamlead-role-id", null, "TeamLead", "TEAMLEAD" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin-role-id");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "developer-role-id");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "teamlead-role-id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4d1343e8-06af-4a41-9d44-cbda3a770602", null, "Admin", "ADMIN" },
                    { "51fa6aa9-274c-4cb7-9354-bcce1943b4a3", null, "Developer", "DEVELOPER" },
                    { "97e3eb47-7185-4b9a-b849-7eb9a1b66cc3", null, "TeamLead", "TEAMLEAD" }
                });
        }
    }
}
