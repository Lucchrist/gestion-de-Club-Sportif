using System;

namespace Stage.Models
{
    public class Statistique
    {
        public string EntrainementTitre { get; set; }
        public DateTime DateEntrainement { get; set; }
        public string MembreNom { get; set; }
        public int Id { get; set; }

        // Total des membres
        public int TotalMembres { get; set; }

        // Nombre de membres ayant participé
        public int MembresPresents { get; set; }

        // Nombre de membres absents
        public int MembresAbsents { get; set; }

        // Nombre de membres excusés
        public int MembresExcuses { get; set; }

        // Pourcentages de participation
        public decimal PourcentagePresence { get; set; }
        public decimal PourcentageAbsence { get; set; }
        public decimal PourcentageExcuses { get; set; }

        // Nom ou identifiant de l'entraînement
       
        public int EntrainementId { get; set; }
        public Entrainement Entrainement { get; set; }
        public string TypeEvenement { get; set; }

        // Date à laquelle les statistiques ont été générées
        public DateTime DateStatistique { get; set; }

        // Statistique par rapport à la période (semaine, mois, etc.)
        public string Periode { get; set; }
    }
}
