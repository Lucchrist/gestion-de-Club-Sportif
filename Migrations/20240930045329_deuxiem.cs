using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class deuxiem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adresse",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "DateNaissance",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "NumeroMembre",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "PreferencesCommunication",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "StatutPaiement",
                table: "Membres");

            migrationBuilder.DropColumn(
                name: "TypeAdhesion",
                table: "Membres");

            migrationBuilder.AddColumn<int>(
                name: "StatutAdhesion",
                table: "Membres",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatutAdhesion",
                table: "Membres");

            migrationBuilder.AddColumn<string>(
                name: "Adresse",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateNaissance",
                table: "Membres",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroMembre",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferencesCommunication",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatutPaiement",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeAdhesion",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
