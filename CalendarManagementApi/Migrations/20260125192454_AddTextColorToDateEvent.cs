using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddTextColorToDateEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TextColor",
                table: "DateEvents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "DateEvents");
        }
    }
}
