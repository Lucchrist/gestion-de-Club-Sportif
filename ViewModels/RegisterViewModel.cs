using System.ComponentModel.DataAnnotations;

namespace Stage.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez fournir une adresse email valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Veuillez confirmer votre mot de passe.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Veuillez fournir un numéro de téléphone valide.")]
        public string Telephone { get; set; }

        public DateTime DateAdhesion { get; set; }

        public string StatutAdhesion { get; set; }

        public string Role { get; set; } = "Membre"; // Rôle par défaut
    }
}
