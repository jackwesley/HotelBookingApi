using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelBooking.Data.Migrations
{
    public partial class RefatoringReservationCreatingStayTimeClass1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckIn",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CheckOut",
                table: "Reservations");

            migrationBuilder.AddColumn<Guid>(
                name: "StayTimeId",
                table: "Reservations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StayTime",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckOut = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StayTime", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StayTimeId",
                table: "Reservations",
                column: "StayTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations",
                column: "StayTimeId",
                principalTable: "StayTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations");

            migrationBuilder.DropTable(
                name: "StayTime");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_StayTimeId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "StayTimeId",
                table: "Reservations");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckIn",
                table: "Reservations",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOut",
                table: "Reservations",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
