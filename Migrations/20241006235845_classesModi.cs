using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class classesModi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisation_Membres_MembreId",
                table: "Cotisation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cotisation",
                table: "Cotisation");

            migrationBuilder.RenameTable(
                name: "Cotisation",
                newName: "Cotisations");

            migrationBuilder.RenameIndex(
                name: "IX_Cotisation_MembreId",
                table: "Cotisations",
                newName: "IX_Cotisations_MembreId");

            migrationBuilder.AlterColumn<string>(
                name: "Telephone",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cotisations",
                table: "Cotisations",
                column: "Id");

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cotisations",
                table: "Cotisations");

            migrationBuilder.RenameTable(
                name: "Cotisations",
                newName: "Cotisation");

            migrationBuilder.RenameIndex(
                name: "IX_Cotisations_MembreId",
                table: "Cotisation",
                newName: "IX_Cotisation_MembreId");

            migrationBuilder.AlterColumn<string>(
                name: "Telephone",
                table: "Membres",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Membres",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cotisation",
                table: "Cotisation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotisation_Membres_MembreId",
                table: "Cotisation",
                column: "MembreId",
                principalTable: "Membres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
