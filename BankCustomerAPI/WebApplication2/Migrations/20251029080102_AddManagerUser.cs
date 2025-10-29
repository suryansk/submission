using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "BankUser_Department",
                schema: "training",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankUser_EmployeeId",
                schema: "training",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankUser_PrimaryBankId",
                schema: "training",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                schema: "training",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerLevel",
                schema: "training",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                schema: "training",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                schema: "training",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "BankUser_PrimaryBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Banks_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "BankUser_PrimaryBankId",
                principalSchema: "training",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Banks_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "PrimaryBankId",
                principalSchema: "training",
                principalTable: "Banks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankUser_Department",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankUser_EmployeeId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankUser_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagerLevel",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                schema: "training",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Banks_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "PrimaryBankId",
                principalSchema: "training",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
