using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stage.Migrations
{
    /// <inheritdoc />
    public partial class statistiquefirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Statistiques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalMembres = table.Column<int>(type: "int", nullable: false),
                    MembresPresents = table.Column<int>(type: "int", nullable: false),
                    MembresAbsents = table.Column<int>(type: "int", nullable: false),
                    MembresExcuses = table.Column<int>(type: "int", nullable: false),
                    PourcentagePresence = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PourcentageAbsence = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PourcentageExcuses = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    EntrainementId = table.Column<int>(type: "int", nullable: false),
                    DateStatistique = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Periode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MembreId = table.Column<int>(type: "int", nullable: true),
                    ParticipationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistiques", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statistiques_Entrainements_EntrainementId",
                        column: x => x.EntrainementId,
                        principalTable: "Entrainements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Statistiques_Membres_MembreId",
                        column: x => x.MembreId,
                        principalTable: "Membres",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Statistiques_Participations_ParticipationId",
                        column: x => x.ParticipationId,
                        principalTable: "Participations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Statistiques_EntrainementId",
                table: "Statistiques",
                column: "EntrainementId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistiques_MembreId",
                table: "Statistiques",
                column: "MembreId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistiques_ParticipationId",
                table: "Statistiques",
                column: "ParticipationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Statistiques");
        }
    }
}
