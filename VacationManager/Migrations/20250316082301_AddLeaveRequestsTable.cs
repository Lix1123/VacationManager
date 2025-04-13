using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VacationManager.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "60d66319-29e5-4bef-81a7-0b5b5f3e17ec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aecf24c7-c5e7-45c8-8fb2-087e94ecb88a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8fd7f8c-0a4e-4bd8-b326-92c71122731e");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "60d66319-29e5-4bef-81a7-0b5b5f3e17ec", null, "Developer", "DEVELOPER" },
                    { "aecf24c7-c5e7-45c8-8fb2-087e94ecb88a", null, "Admin", "ADMIN" },
                    { "d8fd7f8c-0a4e-4bd8-b326-92c71122731e", null, "TeamLead", "TEAMLEAD" }
                });
        }
    }
}
