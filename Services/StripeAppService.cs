using System;
using Stripe;
using Airbnbfinal.Models.Stripe;
using Airbnbfinal.Services.Contracts;

namespace Airbnbfinal.Services
{
    public class StripeAppService : IStripeAppService
    {
        public readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;
        private readonly TokenService _tokenService;

        public StripeAppService(
            ChargeService chargeService, CustomerService customerService, TokenService tokenService)
        {
            _chargeService = chargeService;
            _customerService = customerService;
            _tokenService = tokenService;
        }

        public async Task<StripeCustomer> AddStripeCustomerAsync(AddStripeCustomer customer, CancellationToken ct)
        {
            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = customer.Name,
                    Number = customer.CreditCard.CardNumber,
                    ExpYear = customer.CreditCard.ExpYear,
                    ExpMonth = customer.CreditCard.ExpMonth,
                    Cvc = customer.CreditCard.CVV
                }
            };
            //create new stripe token 

            Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);

            //set customer options using 
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                Source = stripeToken.Id
            };


            Customer createdCustomer = await _customerService.CreateAsync(customerOptions, null, ct);

            return new StripeCustomer(createdCustomer.Name, createdCustomer.Email, createdCustomer.Id);

        }

        public async Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct)
        {
            ChargeCreateOptions paymentOptions = new ChargeCreateOptions
            {
                Customer = payment.userId,
                ReceiptEmail = payment.RecieptEmail,
                Description = payment.Description,
                Currency = payment.Currency,
                Amount = payment.amount

            };
            var createdPayment = await _chargeService.CreateAsync(paymentOptions, null, ct);

            return new StripePayment(
                createdPayment.CustomerId,
                createdPayment.ReceiptEmail,
                createdPayment.Description,
                createdPayment.Currency,
                createdPayment.Amount,
                createdPayment.Id
                );
        }
    }
}
