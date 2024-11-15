using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Stage.Data;
using Stage.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace Stage.Controllers
{
    public class UserController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public UserController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // Méthode pour afficher tous les Users (Index)
        public async Task<IActionResult> Index()
        {
            var Users = await _context.Users.ToListAsync();
            return View(Users);
        }

        // Méthode pour créer un Useristrateur
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(User User)
        {
            if (ModelState.IsValid)
            {
                // Générer un nom d'utilisateur à partir du nom + 3 chiffres aléatoires
                User.Username = GenerateUsername(User.Email);

                // Hash du mot de passe avec BCrypt
                User.Password = HashPassword(User.Password);

                _context.Users.Add(User);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(User);
        }

        // Méthode pour se connecter
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Recherche de l'Useristrateur dans la base de données
            var User = await _context.Users.SingleOrDefaultAsync(a => a.Username == username);

            if (User != null && VerifyPassword(password, User.Password)) // Comparaison des mots de passe
            {
                // Stocker l'utilisateur dans la session de façon sécurisée
                HttpContext.Session.SetString("UserUsername", User.Username);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
            return View();
        }

        // Méthode pour supprimer un Useristrateur
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if (User == null)
            {
                return NotFound();
            }

            _context.Users.Remove(User);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Méthode pour générer un nom d'utilisateur unique (prénom + 3 chiffres)
        private string GenerateUsername(string email)
        {
            var namePart = email.Split('@')[0]; // Extraire le nom avant '@'
            var random = new Random();
            var randomDigits = random.Next(100, 999).ToString(); // Générer 3 chiffres aléatoires
            return $"{namePart}{randomDigits}";
        }

        // Méthode pour hacher le mot de passe avec BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Méthode pour vérifier le mot de passe haché
        private bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
        }
    }
}
