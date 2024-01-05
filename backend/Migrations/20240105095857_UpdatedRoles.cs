using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Admin",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "varchar(32)",
                nullable: false,
                defaultValue: "Default");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
