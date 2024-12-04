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

            // Ajouter les services hébergés pour les rappels
            builder.Services.AddHostedService<RappelParticipationService>(); // Service pour les participations
            builder.Services.AddHostedService<RappelMembreService>(); // Service pour les membres avec statut expiré

            // Ajouter le service Email
            builder.Services.AddScoped<EmailService>();

           

            // Ajouter le DbContext pour la base de données
            builder.Services.AddDbContext<ClubSportifDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClubSportifDbContext")));

            // Ajouter les services Identity pour l'authentification et la gestion des rôles
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ClubSportifDbContext>()
                .AddDefaultTokenProviders();

            // Configuration des sessions
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Délai d'inactivité de 30 minutes
                options.Cookie.HttpOnly = true; // Sécurité des cookies
                options.Cookie.IsEssential = true; // Essentiel pour la session
            });

            // Configurer CORS pour autoriser les requêtes API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Ajouter les services de contrôleurs et vues
            builder.Services.AddControllersWithViews();

            // Ajouter les contrôleurs pour les API
            builder.Services.AddControllers(); // Permet de mapper les API

            var app = builder.Build();

            // Configuration du pipeline des requêtes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error"); // Gérer les exceptions en environnement production
                app.UseHsts(); // Appliquer la politique HSTS pour HTTPS
            }

            app.UseHttpsRedirection(); // Rediriger les requêtes HTTP vers HTTPS
            app.UseStaticFiles(); // Permettre l'accès aux fichiers statiques

            app.UseRouting(); // Configurer le routage

            // Utiliser CORS
            app.UseCors("AllowAll");

            // Utiliser les sessions
            app.UseSession();

            // Ajouter l'authentification et l'autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // Définir la route par défaut
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Définir la route pour les API
            app.MapControllers(); // Permet de mapper les routes API

            // Lancer l'application
            app.Run();
        }
    }
}
