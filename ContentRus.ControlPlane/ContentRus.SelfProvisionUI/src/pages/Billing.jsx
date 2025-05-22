import React, { useState } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import './../styles/billing.css';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY);

export function Billing() {
  const [loading, setLoading] = useState(false);
  
  // Replace with your actual plans and price IDs
  const plans = [
    {
      id: 'basic',
      name: 'Basic Plan',
      price: '$9.99',
      interval: 'month',
      features: ['Create Websites', 'Tenant Management'],
      priceId: import.meta.env.VITE_PRICE_ID_BASIC
    },
    {
      id: 'pro',
      name: 'Pro Plan',
      price: '$40.00',
      interval: 'month',
      features: ['Create Websites', 'Tenant Management', 'More storage space'],
      priceId: import.meta.env.VITE_PRICE_ID_PRO
    },
    {
      id: 'enterprise',
      name: 'Enterprise Plan',
      price: '$60.00',
      interval: 'month',
      features: ['Create Websites', 'Tenant Management', 'More storage space', 'Object storage in cloud', 'Many more...'],
      priceId: import.meta.env.VITE_PRICE_ID_ENTERPRISE
    }
  ];

  const handleSubscribe = async (priceId) => {
    setLoading(true);
    
    try {
      // Call your backend API to create a checkout session
      const response = await fetch('https://49d8-2001-8a0-c787-5f01-e2df-76e5-ea16-2fc9.ngrok-free.app/api/stripe/create-checkout-session', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          priceId: priceId,
        }),
      });
      
      const session = await response.json();
      
      // Redirect to Stripe Checkout
      const stripe = await stripePromise;
      await stripe.redirectToCheckout({
        sessionId: session.id,
      });
    } catch (error) {
      console.error('Error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="App">
      <header>
        <h1>Choose Your Subscription Plan</h1>
      </header>
      
      <div className="pricing-container">
        <div className="pricing-grid">
          {plans.map((plan) => (
            <div key={plan.id} className={`pricing-card`}>
              <h2>{plan.name}</h2>
              <div className="price">
                <span className="amount">{plan.price}</span>
                <span className="interval">/{plan.interval}</span>
              </div>
              <ul className="features">
                {plan.features.map((feature, index) => (
                  <li key={index}>{feature}</li>
                ))}
              </ul>
              <button 
                className="subscribe-button"
                onClick={() => handleSubscribe(plan.priceId)}
                disabled={loading}
              >
                {loading ? 'Processing...' : `Subscribe to ${plan.name}`}
              </button>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}