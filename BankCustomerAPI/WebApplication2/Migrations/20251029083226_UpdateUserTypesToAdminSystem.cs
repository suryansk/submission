using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTypesToAdminSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_BankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Banks_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PrimaryBankId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankId",
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
                name: "EmployeeId",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagerLevel",
                schema: "training",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "BankUser_Department",
                schema: "training",
                table: "Users",
                newName: "Admin_Department");

            migrationBuilder.RenameColumn(
                name: "PrimaryBankId",
                schema: "training",
                table: "Users",
                newName: "Admin_YearsOfExperience");

            migrationBuilder.RenameColumn(
                name: "Designation",
                schema: "training",
                table: "Users",
                newName: "Admin_Position");

            migrationBuilder.AddColumn<string>(
                name: "AdminLevel",
                schema: "training",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAdminActionAt",
                schema: "training",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSystemActionAt",
                schema: "training",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminLevel",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastAdminActionAt",
                schema: "training",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastSystemActionAt",
                schema: "training",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Admin_Department",
                schema: "training",
                table: "Users",
                newName: "BankUser_Department");

            migrationBuilder.RenameColumn(
                name: "Admin_YearsOfExperience",
                schema: "training",
                table: "Users",
                newName: "PrimaryBankId");

            migrationBuilder.RenameColumn(
                name: "Admin_Position",
                schema: "training",
                table: "Users",
                newName: "Designation");

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                schema: "training",
                table: "Users",
                type: "int",
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
                name: "EmployeeId",
                schema: "training",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerLevel",
                schema: "training",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BankId",
                schema: "training",
                table: "Users",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BankUser_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "BankUser_PrimaryBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrimaryBankId",
                schema: "training",
                table: "Users",
                column: "PrimaryBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Banks_BankId",
                schema: "training",
                table: "Users",
                column: "BankId",
                principalSchema: "training",
                principalTable: "Banks",
                principalColumn: "Id");

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
    }
}
