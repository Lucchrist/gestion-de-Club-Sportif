using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateHeure",
                table: "Entrainements",
                newName: "DateFin");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDebut",
                table: "Entrainements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HeureDebut",
                table: "Entrainements",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HeureFin",
                table: "Entrainements",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateDebut",
                table: "Entrainements");

            migrationBuilder.DropColumn(
                name: "HeureDebut",
                table: "Entrainements");

            migrationBuilder.DropColumn(
                name: "HeureFin",
                table: "Entrainements");

            migrationBuilder.RenameColumn(
                name: "DateFin",
                table: "Entrainements",
                newName: "DateHeure");
        }
    }
}
