using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelBooking.Data.Migrations
{
    public partial class PopulatingRoomEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Room",
                columns: new[] { "Id", "Number" },
                values: new object[] { new Guid("539161dd-0ac5-4222-a410-24fbaf7dc70f"), 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Room",
                keyColumn: "Id",
                keyValue: new Guid("539161dd-0ac5-4222-a410-24fbaf7dc70f"));
        }
    }
}
