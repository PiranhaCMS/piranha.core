using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.IO;
using System.Threading.Tasks;
using StripeApi.Models;
using StripeApi.RabbitMQ;

namespace StripeApi.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;
        private readonly RabbitMqPublisher _publisher;
        //private readonly Func<Task<RabbitMqPublisher>> _publisherFactory;

        public WebhookController(IOptions<StripeSettings> stripeOptions, RabbitMqPublisher publisher)
        {
            _stripeSettings = stripeOptions.Value;
            _publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            //Console.WriteLine($"=== WEBHOOK RECEIVED AT {DateTime.UtcNow} ===");
            //Console.WriteLine($"Request Method: {Request.Method}");
            //Console.WriteLine($"Request Path: {Request.Path}");
            //Console.WriteLine($"Content-Type: {Request.ContentType}");
            
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            //Console.WriteLine($"Webhook payload length: {json.Length}");
            //Console.WriteLine($"Webhook payload preview: {json.Substring(0, Math.Min(200, json.Length))}...");
            
            string webhookSecret = _stripeSettings.WebhookSecret;
            //Console.WriteLine($"Using webhook secret: {(!string.IsNullOrEmpty(webhookSecret) ? "SET" : "NOT SET")}");

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    webhookSecret
                );

                //Console.WriteLine($"✅ STRIPE EVENT VERIFIED: {stripeEvent.Type}");
                //Console.WriteLine($"Event ID: {stripeEvent.Id}");

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var checkoutSession = stripeEvent.Data.Object as Session;
                        if (checkoutSession != null)
                        {
                            //Console.WriteLine($"🎉 CHECKOUT COMPLETED: {checkoutSession.Id}");
                            await HandleCheckoutCompleted(checkoutSession);
                        }
                        break;

                    case "customer.subscription.created":
                        var subscriptionCreated = stripeEvent.Data.Object as Subscription;

                        var tenantId = "lol";
                        if (subscriptionCreated != null)
                        {
                            //Console.WriteLine($"📋 SUBSCRIPTION CREATED: {subscriptionCreated.Id}");
                            await HandleSubscriptionCreated(subscriptionCreated,tenantId);
                        }
                        break;

                    case "invoice.payment_succeeded":
                        var invoice = stripeEvent.Data.Object as Invoice;
                        if (invoice != null)
                        {
                            //Console.WriteLine($"💰 PAYMENT SUCCEEDED: {invoice.Id}");
                            await HandleInvoicePaymentSucceeded(invoice);
                        }
                        break;

                    case "customer.subscription.updated":
                        var subscriptionUpdated = stripeEvent.Data.Object as Subscription;
                        if (subscriptionUpdated != null)
                        {
                            //Console.WriteLine($"📝 SUBSCRIPTION UPDATED: {subscriptionUpdated.Id}");
                            await HandleSubscriptionUpdated(subscriptionUpdated);
                        }
                        break;

                    case "customer.subscription.deleted":
                        var subscriptionDeleted = stripeEvent.Data.Object as Subscription;
                        if (subscriptionDeleted != null)
                        {
                            //Console.WriteLine($"❌ SUBSCRIPTION DELETED: {subscriptionDeleted.Id}");
                            await HandleSubscriptionCanceled(subscriptionDeleted);
                        }
                        break;

                    default:
                        Console.WriteLine($"⚠️ UNHANDLED EVENT TYPE: {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Stripe webhook error: {e.Message}");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Webhook processing error: {e.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task HandleCheckoutCompleted(Session checkoutSession)
        {
            Console.WriteLine($"Checkout session completed: {checkoutSession.Id}");
            //Console.WriteLine($"Customer ID: {checkoutSession.CustomerId}");
            //Console.WriteLine($"Subscription ID: {checkoutSession.SubscriptionId}");
        }

        private async Task HandleSubscriptionCreated(Subscription subscription,String tenantId)
        {
            Console.WriteLine($"Subscription created: {subscription.Id}");
            //Console.WriteLine("tenantID in webhooks",tenantId);
            
            try
            {
                var priceId = subscription.Items.Data[0].Price.Id;
                var productId = subscription.Items.Data[0].Price.ProductId;

                // Fetch the product to get its name
                var productService = new ProductService();
                var product = await productService.GetAsync(productId);

                //Console.WriteLine($"🔍 Subscription is for product: {product.Name}");
                var evt = new PaymentConfirmedEvent
                {
                    Plan = product.Name,
                    Status = "subscription created",
                    TenantID = "lol"
                };

                await _publisher.PublishAsync(evt);


                Console.WriteLine("✅ Checkout completion event published successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error publishing checkout completion event: {e.Message}");
            }
        }

        private async Task HandleSubscriptionUpdated(Subscription subscription)
        {
            Console.WriteLine($"Subscription updated: {subscription.Id}, Status: {subscription.Status}");
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