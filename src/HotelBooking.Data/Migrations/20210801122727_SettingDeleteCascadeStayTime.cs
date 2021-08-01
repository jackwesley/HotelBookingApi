using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelBooking.Data.Migrations
{
    public partial class SettingDeleteCascadeStayTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations",
                column: "StayTimeId",
                principalTable: "StayTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_StayTime_StayTimeId",
                table: "Reservations",
                column: "StayTimeId",
                principalTable: "StayTime",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
