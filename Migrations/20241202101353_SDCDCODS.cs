using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class SDCDCODS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisations_Membres_MembreId",
                table: "Cotisations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cotisations",
                table: "Cotisations");

            migrationBuilder.RenameTable(
                name: "Cotisations",
                newName: "Abonnements");

            migrationBuilder.RenameIndex(
                name: "IX_Cotisations_MembreId",
                table: "Abonnements",
                newName: "IX_Abonnements_MembreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Abonnements",
                table: "Abonnements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Abonnements_Membres_MembreId",
                table: "Abonnements",
                column: "MembreId",
                principalTable: "Membres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abonnements_Membres_MembreId",
                table: "Abonnements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Abonnements",
                table: "Abonnements");

            migrationBuilder.RenameTable(
                name: "Abonnements",
                newName: "Cotisations");

            migrationBuilder.RenameIndex(
                name: "IX_Abonnements_MembreId",
                table: "Cotisations",
                newName: "IX_Cotisations_MembreId");

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
    }
}
