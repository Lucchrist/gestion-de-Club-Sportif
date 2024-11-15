using Stripe;
using Stripe.Terminal;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Stage.Models;

namespace Stage.Services
{
    public class StripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        // Créer un ConnectionToken pour le terminal
        // (ajoutez cette méthode si nécessaire)

        // Créer un PaymentIntent pour un paiement par terminal
        public PaymentIntent CreatePaymentIntent(decimal amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe utilise les centimes
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card_present" },
                CaptureMethod = "manual" // Nécessaire pour Stripe Terminal
            };

            var service = new PaymentIntentService();
            return service.Create(options);
        }

        // Capturer un paiement une fois que la transaction est approuvée sur le terminal
        public PaymentIntent CapturePayment(string paymentIntentId)
        {
            var service = new PaymentIntentService();
            return service.Capture(paymentIntentId);
        }

        // Méthode pour obtenir la clé publique Stripe
        public string GetPublicKey()
        {
            return _stripeSettings.PublicKey;
        }
    }
}
