using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absencespot.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class added_workschedule_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndHour",
                table: "WorkSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Hours",
                table: "WorkSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartHour",
                table: "WorkSchedule",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalWorkDays",
                table: "WorkSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkDays",
                table: "WorkSchedule",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "WorkSchedule");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "WorkSchedule");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "WorkSchedule");

            migrationBuilder.DropColumn(
                name: "TotalWorkDays",
                table: "WorkSchedule");

            migrationBuilder.DropColumn(
                name: "WorkDays",
                table: "WorkSchedule");
        }
    }
}
