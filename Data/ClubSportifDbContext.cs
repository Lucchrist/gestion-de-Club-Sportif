using Microsoft.EntityFrameworkCore;
using Stage.Models;

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
        public DbSet<Statistique> Statistiques { get; set; }
     

        public DbSet<Abonnement> Abonnements { get; set; }

        public DbSet<Adhesion> Adhesions { get; set; }



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
