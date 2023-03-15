namespace Airbnbfinal.Models.Stripe
{
    public record AddStripePayment(
        string userId,
        string RecieptEmail,
        string Description,
        string Currency,
        long amount
        );
}
