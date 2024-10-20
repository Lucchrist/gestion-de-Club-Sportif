using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class authentification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Cotisations");

            migrationBuilder.DropColumn(
                name: "PayPalTransactionId",
                table: "Cotisations");

            migrationBuilder.DropColumn(
                name: "StatutCotisation",
                table: "Cotisations");

            migrationBuilder.RenameColumn(
                name: "TypeCotisation",
                table: "Cotisations",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "Membres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "Membres",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "Membres",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "Membres",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "Membres",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Membres",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Membres");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Cotisations",
                newName: "TypeCotisation");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Cotisations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayPalTransactionId",
                table: "Cotisations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatutCotisation",
                table: "Cotisations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
