using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccelokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567843"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 22, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567893"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 16, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567894"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567843"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 3, 21, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567893"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567894"),
                columns: new[] { "CreatedAt", "EventDate", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 28, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
