using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BinaryCity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClientContactId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientContactId",
                table: "ClientContacts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientContactId",
                table: "ClientContacts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
