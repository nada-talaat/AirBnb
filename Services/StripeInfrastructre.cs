using System;
using Stripe;
using Airbnbfinal.Models.Stripe;
using Airbnbfinal.Services.Contracts;

namespace Airbnbfinal.Services
{
    public static class StripeInfrastructre
    {
        public static IServiceCollection AddStripeInfrastructre(this IServiceCollection services,IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeSettings:SecretKey");
            return services
                .AddScoped<CustomerService>()
                .AddScoped<ChargeService>()
                .AddScoped<TokenService>()
                .AddScoped<IStripeAppService, StripeAppService>();
        }
    }
}
