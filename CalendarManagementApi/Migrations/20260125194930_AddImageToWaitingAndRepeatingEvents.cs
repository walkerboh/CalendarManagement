using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToWaitingAndRepeatingEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "WaitingEvents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "RepeatingEvents",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "WaitingEvents");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "RepeatingEvents");
        }
    }
}
