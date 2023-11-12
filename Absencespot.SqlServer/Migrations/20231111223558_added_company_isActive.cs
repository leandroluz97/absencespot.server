using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absencespot.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class added_company_isActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailContact",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Company",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailContact",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Company");
        }
    }
}
