using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blazor_Web_App_Auth.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedusers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "admin@example.com", "admin", "$2a$11$qhy8ZYlhgyJ21BiiI7lQQuqMBI0DiqXZx7KmvE62apomgMNdluQda", "Admin" },
                    { 2, "user@example.com", "testuser", "$2a$11$ZDPEwDgkXqFYnNmeVi8e1uF5rS7IuUCYTvO9iW8uCUjVRckSawLD6", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
