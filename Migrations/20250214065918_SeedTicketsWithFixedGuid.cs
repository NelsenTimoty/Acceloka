using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccelokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedTicketsWithFixedGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookedTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quota = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookedTicketDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookedTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookedTicketDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookedTicketDetails_BookedTickets_BookedTicketId",
                        column: x => x.BookedTicketId,
                        principalTable: "BookedTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookedTicketDetails_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-789a-bcde-f01234567891"), "Sports" },
                    { new Guid("a1b2c3d4-e5f6-789a-bcde-f01234567892"), "Theater" },
                    { new Guid("d1b2c3d4-e5f6-789a-bcde-f01234567890"), "Concert" }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "CategoryId", "Code", "CreatedAt", "CreatedBy", "EventDate", "Name", "Price", "Quota", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567843"), new Guid("a1b2c3d4-e5f6-789a-bcde-f01234567892"), "BM003", new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM", new DateTime(2024, 3, 21, 12, 0, 0, 0, DateTimeKind.Utc), "Hollywood Broadway Musical", 180.00m, 75, new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM" },
                    { new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567893"), new Guid("d1b2c3d4-e5f6-789a-bcde-f01234567890"), "RF001", new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM", new DateTime(2024, 3, 15, 12, 0, 0, 0, DateTimeKind.Utc), "Queens Rock Festival", 150.00m, 100, new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM" },
                    { new Guid("f1b2c3d4-e5f6-789a-bcde-f01234567894"), new Guid("a1b2c3d4-e5f6-789a-bcde-f01234567891"), "BF002", new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM", new DateTime(2024, 2, 28, 12, 0, 0, 0, DateTimeKind.Utc), "NBA Basketball Finals", 200.00m, 50, new DateTime(2024, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), "SYSTEM" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookedTicketDetails_BookedTicketId",
                table: "BookedTicketDetails",
                column: "BookedTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_BookedTicketDetails_TicketId",
                table: "BookedTicketDetails",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Code",
                table: "Tickets",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookedTicketDetails");

            migrationBuilder.DropTable(
                name: "BookedTickets");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
