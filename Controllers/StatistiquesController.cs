using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stage.Data;
using Stage.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Stage.Controllers
{
    public class StatistiquesController : Controller
    {
        private readonly ClubSportifDbContext _context;

        public StatistiquesController(ClubSportifDbContext context)
        {
            _context = context;
        }

        // GET: Statistiques
        public async Task<IActionResult> Index(string periode = "mois")
        {
            DateTime startDate;
            DateTime endDate = DateTime.Now;

            switch (periode)
            {
                case "semaine":
                    startDate = endDate.AddDays(-7);
                    break;
                case "mois":
                    startDate = endDate.AddMonths(-1);
                    break;
                case "annee":
                    startDate = endDate.AddYears(-1);
                    break;
                case "tout":
                    startDate = DateTime.MinValue; // Pour afficher toutes les périodes
                    break;
                default:
                    startDate = endDate.AddMonths(-1);
                    break;
            }

            var statistiques = await _context.Participations
                .Include(p => p.Entrainement)
                .Include(p => p.Membre)
                .Where(p => p.Entrainement.DateDebut >= startDate && p.Entrainement.DateDebut <= endDate)
                .GroupBy(p => p.EntrainementId)
                .Select(g => new Statistique
                {
                    EntrainementId = g.Key,
                    EntrainementTitre = g.First().Entrainement.Titre,
                    DateEntrainement = g.First().Entrainement.DateDebut,
                    TypeEvenement = g.First().Entrainement.TypeEvenement,  // Ajout du TypeEvenement
                    MembresPresents = g.Count(pa => pa.StatutParticipation == "Présent"),
                    MembresAbsents = g.Count(pa => pa.StatutParticipation == "Absent"),
                    MembresExcuses = g.Count(pa => pa.StatutParticipation == "Excusé"),
                    TotalMembres = g.Count(),
                    PourcentagePresence = (g.Count(pa => pa.StatutParticipation == "Présent") * 100m) / g.Count(),
                    PourcentageAbsence = (g.Count(pa => pa.StatutParticipation == "Absent") * 100m) / g.Count(),
                    PourcentageExcuses = (g.Count(pa => pa.StatutParticipation == "Excusé") * 100m) / g.Count(),
                    DateStatistique = DateTime.Now,
                    Periode = periode
                }).ToListAsync();

            ViewBag.Periode = periode;
            return View(statistiques);
        }



        // Exporter les statistiques en CSV
        public IActionResult ExportToCSV()
        {
            var statistiques = _context.Participations
                .Include(p => p.Entrainement)
                .Include(p => p.Membre)
                .Select(p => new
                {
                    Entrainement = p.Entrainement.Titre,
                    Date = p.Entrainement.DateDebut,
                    Membre = p.Membre.Nom,
                    Statut = p.StatutParticipation
                }).ToList();

            var builder = new StringBuilder();
            builder.AppendLine("Entraînement,Date,Membre,Statut");

            foreach (var stat in statistiques)
            {
                builder.AppendLine($"{stat.Entrainement},{stat.Date:yyyy-MM-dd},{stat.Membre},{stat.Statut}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "statistiques_participation.csv");
        }

        // Exporter les statistiques en PDF
        public IActionResult ExportToPDF()
        {
            var statistiques = _context.Participations
                .Include(p => p.Entrainement)
                .Include(p => p.Membre)
                .Select(p => new
                {
                    Entrainement = p.Entrainement.Titre,
                    Date = p.Entrainement.DateDebut,
                    Membre = p.Membre.Nom,
                    Statut = p.StatutParticipation
                }).ToList();

            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Statistiques de Participation"));

                Table table = new Table(4);
                table.AddCell("Entraînement");
                table.AddCell("Date");
                table.AddCell("Membre");
                table.AddCell("Statut");

                foreach (var stat in statistiques)
                {
                    table.AddCell(stat.Entrainement);
                    table.AddCell(stat.Date.ToString("yyyy-MM-dd"));
                    table.AddCell(stat.Membre);
                    table.AddCell(stat.Statut);
                }

                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", "statistiques_participation.pdf");
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var participationData = await _context.Participations
                .Include(p => p.Entrainement)
                .Include(p => p.Membre)
                .Where(p => p.EntrainementId == id)
                .ToListAsync();

            if (participationData == null || !participationData.Any())
            {
                return NotFound();
            }

            var entrainement = participationData.First().Entrainement;

            var statistique = new Statistique
            {
                EntrainementId = entrainement.Id,
                EntrainementTitre = entrainement.Titre,
                DateEntrainement = entrainement.DateDebut,
                MembresPresents = participationData.Count(p => p.StatutParticipation == "Présent"),
                MembresAbsents = participationData.Count(p => p.StatutParticipation == "Absent"),
                MembresExcuses = participationData.Count(p => p.StatutParticipation == "Excusé"),
                TotalMembres = participationData.Count(),
                PourcentagePresence = (participationData.Count(p => p.StatutParticipation == "Présent") * 100m) / participationData.Count(),
                PourcentageAbsence = (participationData.Count(p => p.StatutParticipation == "Absent") * 100m) / participationData.Count(),
                PourcentageExcuses = (participationData.Count(p => p.StatutParticipation == "Excusé") * 100m) / participationData.Count()
            };

            return View(statistique);
        }

        // Téléverser un fichier CSV pour ajouter des statistiques
        [HttpPost]
        public IActionResult UploadCSV(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("", "Aucun fichier sélectionné");
                return RedirectToAction(nameof(Index));
            }

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ","
                };
                using (var csvReader = new CsvHelper.CsvReader(streamReader, csvConfig)) // Préciser CsvHelper.CsvReader
                {
                    var participations = csvReader.GetRecords<Participation>().ToList();
                    foreach (var participation in participations)
                    {
                        _context.Participations.Add(participation);
                    }
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
