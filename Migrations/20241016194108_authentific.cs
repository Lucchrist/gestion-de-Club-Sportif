using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class authentific : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisations_Membres_MembreId",
                table: "Cotisations");

            migrationBuilder.DropIndex(
                name: "IX_Cotisations_MembreId",
                table: "Cotisations");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Membres");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotisations_Membres_MembreId1",
                table: "Cotisations");

            migrationBuilder.DropIndex(
                name: "IX_Cotisations_MembreId1",
                table: "Cotisations");

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

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Membres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
