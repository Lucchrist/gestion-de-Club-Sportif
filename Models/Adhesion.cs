using System;
using System.ComponentModel.DataAnnotations;

namespace Stage.Models
{
    public class Adhesion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MembreId { get; set; }
        public Membre Membre { get; set; }

        [Required]
        [StringLength(20)]
        public string TypeAbonnement { get; set; } // Mensuel ou Annuel

        [Required]
        [DataType(DataType.Currency)]
        public decimal Montant { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [StringLength(50)]
        public string Statut { get; set; }
        

    }
}
