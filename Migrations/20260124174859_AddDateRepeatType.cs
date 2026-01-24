using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDateRepeatType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "RepeatingEvents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "RepeatingEvents",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "RepeatingEvents");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "RepeatingEvents");
        }
    }
}
