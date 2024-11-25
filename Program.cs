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

            // Ajouter le service Stripe
            builder.Services.AddSingleton<StripeService>();
            builder.Services.AddScoped<EmailService>();


            // Ajouter le service pour les emails
            builder.Services.AddTransient<EmailService>();
            builder.Services.AddScoped<EmailService>();


            // Ajouter le service pour les abonnements
            builder.Services.AddScoped<AbonnementService>();

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
                options.Cookie.IsEssential = true;
            });

            // Configurer les options d'authentification
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Login"; // Redirection vers la page de connexion
                options.AccessDeniedPath = "/Home/AccessDenied"; // Redirection en cas d'accès refusé
            });

            // Ajouter les services de contrôleurs et vues
            builder.Services.AddControllersWithViews();

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

            // Utiliser les sessions
            app.UseSession();

            // Ajouter l'authentification et l'autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // Définir la route par défaut
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Lancer l'application
            app.Run();
        }
    }
}
