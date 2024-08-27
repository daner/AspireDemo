using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireDemo.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sender",
                schema: "AspireDemo",
                table: "Messages",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "Receiever",
                schema: "AspireDemo",
                table: "Messages",
                newName: "Room");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                schema: "AspireDemo",
                table: "Messages",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "AspireDemo",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Username",
                schema: "AspireDemo",
                table: "Messages",
                newName: "Sender");

            migrationBuilder.RenameColumn(
                name: "Room",
                schema: "AspireDemo",
                table: "Messages",
                newName: "Receiever");
        }
    }
}
