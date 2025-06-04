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

                        if (subscriptionCreated != null)
                        {
                            //Console.WriteLine($"📋 SUBSCRIPTION CREATED: {subscriptionCreated.Id}");
                            await HandleSubscriptionCreated(subscriptionCreated);
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

            // Get tenantId from checkout session metadata
            string tenantId = null;
            string productName = null;
            if (checkoutSession.Metadata != null && checkoutSession.Metadata.TryGetValue("tenantId", out var storedTenantId))
            {
                tenantId = storedTenantId;
            }

            try
            {
                if (!string.IsNullOrEmpty(checkoutSession.SubscriptionId))
                {
                    var subscriptionService = new SubscriptionService();
                    var subscription = await subscriptionService.GetAsync(checkoutSession.SubscriptionId);

                    var priceId = subscription.Items.Data[0].Price.Id;
                    var productId = subscription.Items.Data[0].Price.ProductId;

                    var productService = new ProductService();
                    var product = await productService.GetAsync(productId);
                    productName = product.Name;

                    Console.WriteLine($"Tenant ID: {tenantId}");
                    Console.WriteLine($"Subscription ID: {subscription.Id}");
                    Console.WriteLine($"Plan Name: {product.Name}");
                }
                else
                {
                    Console.WriteLine("⚠️ No subscription associated with checkout session");
                }

                var evt = new PaymentConfirmedEvent
                    {
                        Type = "payment",
                        Plan = productName,
                        Status = "subscription created",
                        TenantID = tenantId ?? "unknown"
                    };

                await _publisher.PublishAsync(evt);

                Console.WriteLine("✅ Checkout completion event published successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error publishing checkout completion event: {e.Message}");
            } 
        }

        private async Task HandleSubscriptionCreated(Subscription subscription)
        {
            
            /* string tenantId = null;
            try
            {
                if (subscription.LatestInvoiceId != null)
                {
                    var invoiceService = new InvoiceService();
                    var invoice = await invoiceService.GetAsync(subscription.LatestInvoiceId);

                    if (invoice.CheckoutSession != null)
                    {
                        var sessionService = new SessionService();
                        var checkoutSession = await sessionService.GetAsync(invoice.CheckoutSession);

                        if (checkoutSession.Metadata != null && checkoutSession.Metadata.TryGetValue("tenantId", out var storedTenantId))
                        {
                            tenantId = storedTenantId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tenantId from checkout session metadata: {ex.Message}");
            }

            Console.WriteLine($"Subscription created: {subscription.Id}, tenantId: {tenantId ?? "NOT FOUND"}");
            
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
                    Type = "payment",
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
            } */
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