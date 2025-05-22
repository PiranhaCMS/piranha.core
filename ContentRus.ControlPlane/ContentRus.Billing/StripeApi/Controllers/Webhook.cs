using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StripeApi.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;

        public WebhookController(IOptions<StripeSettings> stripeOptions)
        {
            _stripeSettings = stripeOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            string webhookSecret = _stripeSettings.WebhookSecret;

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                switch (stripeEvent.Type)
                {
                    case "customer.subscription.created":
                        var subscriptionCreated = stripeEvent.Data.Object as Subscription;
                        await HandleSubscriptionCreated(subscriptionCreated);
                        break;

                    case "customer.subscription.updated":
                        var subscriptionUpdated = stripeEvent.Data.Object as Subscription;
                        await HandleSubscriptionUpdated(subscriptionUpdated);
                        break;

                    case "customer.subscription.deleted":
                        var subscriptionDeleted = stripeEvent.Data.Object as Subscription;
                        await HandleSubscriptionCanceled(subscriptionDeleted);
                        break;

                    case "invoice.payment_succeeded":
                        var invoice = stripeEvent.Data.Object as Invoice;
                        await HandleInvoicePaymentSucceeded(invoice);
                        break;

                    case "invoice.payment_failed":
                        var failedInvoice = stripeEvent.Data.Object as Invoice;
                        await HandleInvoicePaymentFailed(failedInvoice);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

        private Task HandleSubscriptionCreated(Subscription subscription)
        {
            Console.WriteLine($"Subscription created: {subscription.Id}");
            return Task.CompletedTask;
        }

        private Task HandleSubscriptionUpdated(Subscription subscription)
        {
            Console.WriteLine($"Subscription updated: {subscription.Id}, Status: {subscription.Status}");
            return Task.CompletedTask;
        }

        private Task HandleSubscriptionCanceled(Subscription subscription)
        {
            Console.WriteLine($"Subscription canceled: {subscription.Id}");
            return Task.CompletedTask;
        }

        private Task HandleInvoicePaymentSucceeded(Invoice invoice)
        {
            Console.WriteLine($"Payment succeeded for invoice: {invoice.Id}");
            return Task.CompletedTask;
        }

        private Task HandleInvoicePaymentFailed(Invoice invoice)
        {
            Console.WriteLine($"Payment failed for invoice: {invoice.Id}");
            return Task.CompletedTask;
        }
    }
}
