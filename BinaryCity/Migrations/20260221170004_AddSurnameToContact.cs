using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryCity.Migrations
{
    /// <inheritdoc />
    public partial class AddSurnameToContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Contacts",
                newName: "Surname");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Contacts",
                newName: "Phone");
        }
    }
}
