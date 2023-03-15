namespace Airbnbfinal.Models.Stripe
{
   public record StripePayment(
       string userId,
       string RecieptEmail,
       string Description,
       string Currency,
       long Amount,
       string PaymentId
       
       );
}
