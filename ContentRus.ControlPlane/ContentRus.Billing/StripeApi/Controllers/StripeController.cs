using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StripeApi.Controllers
{
    [ApiController]
    [Route("api/stripe")]
    public class StripeController(IOptions<StripeSettings> stripeOptions) : ControllerBase
    {
       private readonly StripeSettings _stripeSettings = stripeOptions.Value;

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Backend is working!");
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            Console.WriteLine("id do plan",request);
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = request.PriceId,
                        Quantity = 1,
                    },
                },
                Mode = "subscription",
                SuccessUrl = "http://selfprovision/success?planId=" + request.Id,
                CancelUrl = "http://selfprovision/billing",
                Metadata = new Dictionary<string, string>
                {
                    { "tenantId", request.TenantId } // Associe o tenantId
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return Ok(new { Id = session.Id });
        }
    }

    public class CheckoutRequest
    {
        public string PriceId { get; set; }
        public string TenantId { get; set; }
        public string Id {get; set;}
    }
}