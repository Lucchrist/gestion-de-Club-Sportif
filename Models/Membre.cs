using System;
using System.ComponentModel.DataAnnotations;

namespace Stage.Models
{
    public class Membre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez fournir une adresse email valide.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Veuillez fournir un numéro de téléphone valide.")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "La date d'adhésion est obligatoire.")]
        public DateTime DateAdhesion { get; set; }

        [Required(ErrorMessage = "Le statut d'adhésion est obligatoire.")]
        public string StatutAdhesion { get; set; }
    }

    
}
