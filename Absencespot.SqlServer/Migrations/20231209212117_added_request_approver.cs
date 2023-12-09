using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absencespot.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class added_request_approver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AspNetUsers_UserId",
                table: "Request");

            migrationBuilder.AddColumn<int>(
                name: "ApproverId",
                table: "Request",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OnBehalfOfId",
                table: "Request",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_ApproverId",
                table: "Request",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_OnBehalfOfId",
                table: "Request",
                column: "OnBehalfOfId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AspNetUsers_ApproverId",
                table: "Request",
                column: "ApproverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AspNetUsers_OnBehalfOfId",
                table: "Request",
                column: "OnBehalfOfId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AspNetUsers_UserId",
                table: "Request",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_AspNetUsers_ApproverId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_AspNetUsers_OnBehalfOfId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_AspNetUsers_UserId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_ApproverId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_OnBehalfOfId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "OnBehalfOfId",
                table: "Request");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_AspNetUsers_UserId",
                table: "Request",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
