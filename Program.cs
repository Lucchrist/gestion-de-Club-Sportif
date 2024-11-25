using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Stage.Data;
using Stage.Services;
using Stage.Models;

namespace Stage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration Stripe
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddSingleton<StripeService>(); // Service Stripe

            // Ajouter les services h�berg�s pour les rappels
            builder.Services.AddHostedService<RappelParticipationService>(); //// Service pour les participations


            builder.Services.AddHostedService<RappelMembreService>(); // Service pour les membres avec statut expir�
           
           // builder.Services.AddHostedService<EmailTestBackgroundService>(); // Service pour tester les emails

            // Ajouter le service Email
            builder.Services.AddScoped<EmailService>();

            // Ajouter le service pour les abonnements
            builder.Services.AddScoped<AbonnementService>();

            // Ajouter le DbContext pour la base de donn�es
            builder.Services.AddDbContext<ClubSportifDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClubSportifDbContext")));

            // Ajouter les services Identity pour l'authentification et la gestion des r�les
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ClubSportifDbContext>()
                .AddDefaultTokenProviders();

            // Configuration des sessions
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // D�lai d'inactivit� de 30 minutes
                options.Cookie.HttpOnly = true; // S�curit� des cookies
                options.Cookie.IsEssential = true; // Essentiel pour la session
            });

            // Configurer les options d'authentification
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Login"; // Redirection vers la page de connexion
                options.AccessDeniedPath = "/Home/AccessDenied"; // Redirection en cas d'acc�s refus�
            });

            // Ajouter les services de contr�leurs et vues
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuration du pipeline des requ�tes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error"); // G�rer les exceptions en environnement production
                app.UseHsts(); // Appliquer la politique HSTS pour HTTPS
            }

            app.UseHttpsRedirection(); // Rediriger les requ�tes HTTP vers HTTPS
            app.UseStaticFiles(); // Permettre l'acc�s aux fichiers statiques

            app.UseRouting(); // Configurer le routage

            // Utiliser les sessions
            app.UseSession();

            // Ajouter l'authentification et l'autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // D�finir la route par d�faut
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Lancer l'application
            app.Run();
        }
    }
}
