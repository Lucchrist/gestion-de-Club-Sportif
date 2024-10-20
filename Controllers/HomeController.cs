using Microsoft.AspNetCore.Mvc;
using Stage.Data;
using Stage.Models;
using System.Diagnostics;
using System.Linq;

namespace Stage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public HomeController(ClubSportifDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Charger seulement des statistiques spécifiques, par exemple les statistiques du dernier mois
            var statistiques = _context.Participations
                .Where(p => p.Entrainement.DateDebut >= DateTime.Now.AddMonths(-1))
                .GroupBy(p => p.EntrainementId)
                .Select(g => new Statistique
                {
                    EntrainementId = g.Key,
                    EntrainementTitre = g.First().Entrainement.Titre,
                    DateEntrainement = g.First().Entrainement.DateDebut,
                    MembresPresents = g.Count(pa => pa.StatutParticipation == "Présent"),
                    MembresAbsents = g.Count(pa => pa.StatutParticipation == "Absent"),
                    MembresExcuses = g.Count(pa => pa.StatutParticipation == "Excusé")
                }).Take(5) // Limiter à 5 entrainements pour l'affichage sur la page d'accueil
                .ToList();

            return View(statistiques);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}