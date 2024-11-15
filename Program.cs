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

            // Configuration de Stripe
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            // Ajout du service Stripe
            builder.Services.AddSingleton<StripeService>();

            // Service pour les emails
            builder.Services.AddTransient<EmailService>();

            // Service pour les abonnements
            builder.Services.AddScoped<AbonnementService>();

            // Ajout du DbContext
            builder.Services.AddDbContext<ClubSportifDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClubSportifDbContext")));

            // Ajouter les services Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ClubSportifDbContext>()
                .AddDefaultTokenProviders();

            // Configuration des sessions
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Configurer les param�tres d'authentification (ex. : redirection vers la page de connexion)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Login";  // Redirige vers cette page si l'utilisateur n'est pas connect�
                options.AccessDeniedPath = "/Home/AccessDenied"; // Redirige ici si l'acc�s est refus�
            });

            // Ajout des services de contr�leurs et de vues
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configuration du pipeline des requ�tes HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Utiliser les sessions
            app.UseSession();

            // Utiliser l'authentification et l'autorisation
            app.UseAuthentication();
            app.UseAuthorization();

            // D�finir la route par d�faut
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
