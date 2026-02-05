using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLayerEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TextColor",
                table: "MessagesOfTheDay",
                newName: "Layer");

            migrationBuilder.AddColumn<int>(
                name: "Layer",
                table: "WaitingEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Layer",
                table: "RepeatingEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Layer",
                table: "WaitingEvents");

            migrationBuilder.DropColumn(
                name: "Layer",
                table: "RepeatingEvents");

            migrationBuilder.RenameColumn(
                name: "Layer",
                table: "MessagesOfTheDay",
                newName: "TextColor");
        }
    }
}
