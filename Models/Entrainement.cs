using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stage.Models
{
    public class Entrainement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Titre { get; set; }

        public string Description { get; set; }

        // Nouveau pour gérer la période de l'événement
        [Required(ErrorMessage = "La date de début est obligatoire.")]
        public DateTime DateDebut { get; set; }

        [Required(ErrorMessage = "La date de fin est obligatoire.")]
        public DateTime DateFin { get; set; }

        // Gestion des heures de début et de fin
        [Required(ErrorMessage = "L'heure de début est obligatoire.")]
        public TimeSpan HeureDebut { get; set; }

        [Required(ErrorMessage = "L'heure de fin est obligatoire.")]
        public TimeSpan HeureFin { get; set; }

        [Required(ErrorMessage = "Le lieu est obligatoire.")]
        public string Lieu { get; set; }

        [Required(ErrorMessage = "Le type d'événement est obligatoire.")]
        public string TypeEvenement { get; set; }

        public List<Participation> Participations { get; set; } = new List<Participation>();
        public List<Statistique> Statistiques { get; set; } = new List<Statistique>();
    }
}
