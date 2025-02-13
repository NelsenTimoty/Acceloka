using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccelokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddingMore1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedTicketDetails_BookedTickets_BookedTicketId",
                table: "BookedTicketDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookedTicketId",
                table: "BookedTicketDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedTicketDetails_BookedTickets_BookedTicketId",
                table: "BookedTicketDetails",
                column: "BookedTicketId",
                principalTable: "BookedTickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedTicketDetails_BookedTickets_BookedTicketId",
                table: "BookedTicketDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookedTicketId",
                table: "BookedTicketDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedTicketDetails_BookedTickets_BookedTicketId",
                table: "BookedTicketDetails",
                column: "BookedTicketId",
                principalTable: "BookedTickets",
                principalColumn: "Id");
        }
    }
}
