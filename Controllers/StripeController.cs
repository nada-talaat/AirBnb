﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Airbnbfinal.Models.Stripe;
using Airbnbfinal.Services.Contracts;

namespace Airbnbfinal.Controllers
{
    [Route ("api/[controller]")]
    public class StripeController : Controller
    {
        private readonly IStripeAppService _stripeService;

        public StripeController (IStripeAppService stripeService)
        {
            _stripeService = stripeService;
        }
        [HttpPost("customer/add")]
        public async Task<ActionResult<StripeCustomer>> AddStripeCustomer([FromBody]AddStripeCustomer customer, CancellationToken ct)
        {
            StripeCustomer createdCustomer = await _stripeService.AddStripeCustomerAsync(customer, ct);
            return StatusCode(StatusCodes.Status200OK, createdCustomer);

        }
        [HttpPost("payment/add")]
        public async Task<ActionResult<StripePayment>> AddStripePayment([FromBody]AddStripePayment payment,CancellationToken ct)
        {
            StripePayment createdPayment = await _stripeService.AddStripePaymentAsync(payment, ct);
            return StatusCode(StatusCodes.Status200OK,createdPayment);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
