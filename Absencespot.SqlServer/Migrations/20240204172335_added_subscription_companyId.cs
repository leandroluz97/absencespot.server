using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Absencespot.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class added_subscription_companyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Subscription_SubcriptionId",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_SubcriptionId",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "SubcriptionId",
                table: "Company");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Subscription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Subscription",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionId",
                table: "Subscription",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_CompanyId",
                table: "Subscription",
                column: "CompanyId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Company_CompanyId",
                table: "Subscription",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Company_CompanyId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_CompanyId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "Subscription");

            migrationBuilder.AddColumn<int>(
                name: "SubcriptionId",
                table: "Company",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Company_SubcriptionId",
                table: "Company",
                column: "SubcriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Subscription_SubcriptionId",
                table: "Company",
                column: "SubcriptionId",
                principalTable: "Subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
