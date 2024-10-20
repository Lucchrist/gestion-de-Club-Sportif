using System.ComponentModel.DataAnnotations;

namespace Stage.Models
{
    public class Participation
    {
        public int Id { get; set; }

        // Changement du type MembreId à string pour correspondre à l'Id de IdentityUser
        public int MembreId { get; set; }

        // Relation avec Membre
        public Membre Membre { get; set; }

        public int EntrainementId { get; set; }
        public Entrainement Entrainement { get; set; }

        public string StatutParticipation { get; set; }
        public List<Statistique> Statistiques { get; set; } = new List<Statistique>();
    }
}
