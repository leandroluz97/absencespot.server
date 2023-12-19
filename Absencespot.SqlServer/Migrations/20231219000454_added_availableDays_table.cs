using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absencespot.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class added_availableDays_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailableLeave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AvailableDays = table.Column<double>(type: "float(3)", precision: 3, scale: 2, nullable: false),
                    Period = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AbsenceId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    GlobalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableLeave", x => x.Id);
                    table.UniqueConstraint("AK_AvailableLeave_GlobalId", x => x.GlobalId);
                    table.ForeignKey(
                        name: "FK_AvailableLeave_Absence_AbsenceId",
                        column: x => x.AbsenceId,
                        principalTable: "Absence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvailableLeave_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableLeave_AbsenceId",
                table: "AvailableLeave",
                column: "AbsenceId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableLeave_UserId",
                table: "AvailableLeave",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableLeave");
        }
    }
}
