using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stage.Models
{
    public class Abonnement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Membre")]
        public int MembreId { get; set; }
        public Membre Membre { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeAbonnement { get; set; }

        [Required]
        [StringLength(20)]
        public string Statut { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Montant { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [StringLength(500)]
        public string Commentaire { get; set; }
    }
}
