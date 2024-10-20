using System;
using System.ComponentModel.DataAnnotations;

namespace Stage.Models
{
    public class Cotisation
    {
        public int Id { get; set; }

        [Required]
        public int MembreId { get; set; }
        public Membre Membre { get; set; }

        [Required]
        public decimal Montant { get; set; }

        [Required]
        public string Type { get; set; } // "Mensuel", "Hebdomadaire", "Annuel"

        [Required]
        public DateTime DatePaiement { get; set; }

        [Required]
        public DateTime DateExpiration { get; set; }

        public bool EstExpiree => DateTime.Now > DateExpiration;
    }
}
