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
            builder.Services.AddHostedService<RappelParticipationService>(); // Service pour les participations
            builder.Services.AddHostedService<RappelMembreService>(); // Service pour les membres avec statut expir�

            // Ajouter le service Email
            builder.Services.AddScoped<EmailService>();

           

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

            // Configurer CORS pour autoriser les requ�tes API
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Ajouter les services de contr�leurs et vues
            builder.Services.AddControllersWithViews();

            // Ajouter les contr�leurs pour les API
            builder.Services.AddControllers(); // Permet de mapper les API

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

            // Utiliser CORS
            app.UseCors("AllowAll");

            // Utiliser les sessions
            app.UseSession();

            // Ajouter l'authentification et l'autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // D�finir la route par d�faut
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // D�finir la route pour les API
            app.MapControllers(); // Permet de mapper les routes API

            // Lancer l'application
            app.Run();
        }
    }
}
