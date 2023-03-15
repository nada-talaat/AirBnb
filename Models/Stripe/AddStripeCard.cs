using System;

namespace Airbnbfinal.Models.Stripe
{
   public record AddStripeCard(
            string Name,
            string CardNumber,
            string ExpMonth,
            string ExpYear,
            string CVV );
    
}
