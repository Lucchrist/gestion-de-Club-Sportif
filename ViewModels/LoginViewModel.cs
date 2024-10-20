using System.ComponentModel.DataAnnotations;

namespace Stage.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'adresse email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez fournir une adresse email valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirmer Le mot de passe .")]
        [DataType(DataType.Password)]

        public bool RememberMe { get; set; }
    }
}
