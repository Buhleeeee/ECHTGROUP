using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECHTGROUP.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicsToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "General");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "ChatMessages");
        }
    }
}
