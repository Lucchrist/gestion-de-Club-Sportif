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
        

    }
}
