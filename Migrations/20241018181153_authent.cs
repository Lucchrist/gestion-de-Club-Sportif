using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class authent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisations_Membres_MembreId1",
                table: "Cotisations");

            migrationBuilder.DropIndex(
                name: "IX_Cotisations_MembreId1",
                table: "Cotisations");

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

            migrationBuilder.DropColumn(
                name: "MembreId1",
                table: "Cotisations");

            migrationBuilder.AlterColumn<int>(
                name: "MembreId",
                table: "Participations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Membres",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_Cotisations_MembreId",
                table: "Cotisations",
                column: "MembreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotisations_Membres_MembreId",
                table: "Cotisations",
                column: "MembreId",
                principalTable: "Membres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisations_Membres_MembreId",
                table: "Cotisations");

            migrationBuilder.DropIndex(
                name: "IX_Cotisations_MembreId",
                table: "Cotisations");

            migrationBuilder.AlterColumn<string>(
                name: "MembreId",
                table: "Participations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Membres",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

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

            migrationBuilder.AddColumn<string>(
                name: "MembreId1",
                table: "Cotisations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotisations_MembreId1",
                table: "Cotisations",
                column: "MembreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotisations_Membres_MembreId1",
                table: "Cotisations",
                column: "MembreId1",
                principalTable: "Membres",
                principalColumn: "Id");
        }
    }
}
