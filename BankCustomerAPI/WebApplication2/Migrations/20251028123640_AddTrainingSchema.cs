using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "training");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRoles",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "UserCredentials",
                newName: "UserCredentials",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Roles",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "RolePermissions",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "Permissions",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "GuardianRelationships",
                newName: "GuardianRelationships",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currencies",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Branches",
                newName: "Branches",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Banks",
                newName: "Banks",
                newSchema: "training");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Accounts",
                newSchema: "training");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "training",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "training",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserCredentials",
                schema: "training",
                newName: "UserCredentials");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "training",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                schema: "training",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                schema: "training",
                newName: "Permissions");

            migrationBuilder.RenameTable(
                name: "GuardianRelationships",
                schema: "training",
                newName: "GuardianRelationships");

            migrationBuilder.RenameTable(
                name: "Currencies",
                schema: "training",
                newName: "Currencies");

            migrationBuilder.RenameTable(
                name: "Branches",
                schema: "training",
                newName: "Branches");

            migrationBuilder.RenameTable(
                name: "Banks",
                schema: "training",
                newName: "Banks");

            migrationBuilder.RenameTable(
                name: "Accounts",
                schema: "training",
                newName: "Accounts");
        }
    }
}
