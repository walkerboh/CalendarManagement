using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarManagement.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateEventToMessageOfTheDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "DateEvents",
                newName: "MessagesOfTheDay");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "MessagesOfTheDay",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "PK_DateEvents",
                table: "MessagesOfTheDay",
                newName: "PK_MessagesOfTheDay");

            migrationBuilder.CreateIndex(
                name: "IX_MessagesOfTheDay_Month_Day",
                table: "MessagesOfTheDay",
                columns: new[] { "Month", "Day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessagesOfTheDay_Month_Day",
                table: "MessagesOfTheDay");

            migrationBuilder.RenameIndex(
                name: "PK_MessagesOfTheDay",
                table: "MessagesOfTheDay",
                newName: "PK_DateEvents");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "MessagesOfTheDay",
                newName: "Name");

            migrationBuilder.RenameTable(
                name: "MessagesOfTheDay",
                newName: "DateEvents");
        }
    }
}
