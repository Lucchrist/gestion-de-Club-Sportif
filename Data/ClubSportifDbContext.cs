using Stage.Models;
using Microsoft.EntityFrameworkCore;

namespace Stage.Data
{
    public class ClubSportifDbContext : DbContext
    {
        public ClubSportifDbContext(DbContextOptions<ClubSportifDbContext> options)
            : base(options)
        {
        }

        // Tables du contexte
        public DbSet<Membre> Membres { get; set; }
        public DbSet<Entrainement> Entrainements { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<Cotisation> Cotisations { get; set; } // Ajout de la table Cotisations
        public DbSet<Statistique> Statistiques { get; set; } // Ajout pour la table des statistiques

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relation entre Membre et Participation
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Membre)
                .WithMany(m => m.Participations)
                .HasForeignKey(p => p.MembreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre Entrainement et Participation
            modelBuilder.Entity<Participation>()
                .HasOne(p => p.Entrainement)
                .WithMany(e => e.Participations)
                .HasForeignKey(p => p.EntrainementId);

            // Configuration de la précision du montant dans Cotisation
            modelBuilder.Entity<Cotisation>()
                .Property(c => c.Montant)
                .HasColumnType("decimal(18,2)");

            // Ajout de la configuration pour la table Statistiques (exemple)
            modelBuilder.Entity<Statistique>()
                .Property(s => s.PourcentagePresence)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Statistique>()
                .Property(s => s.PourcentageAbsence)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Statistique>()
                .Property(s => s.PourcentageExcuses)
                .HasColumnType("decimal(5,2)");
        }
    }
}
