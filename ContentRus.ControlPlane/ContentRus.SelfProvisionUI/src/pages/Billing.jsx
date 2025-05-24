import React, { useState } from 'react';
import { loadStripe } from '@stripe/stripe-js';
import { useQuery } from '@tanstack/react-query';
import { API_URL } from '../components/ApiUrl';
import './../styles/billing.css';

const stripePromise = loadStripe(import.meta.env.VITE_STRIPE_PUBLISHABLE_KEY);

export function Billing() {
  const [loading, setLoading] = useState(false);
  const token = localStorage.getItem('token');
  
  // Fetch plans from API
  const { data: plans, isLoading: plansLoading, error: plansError } = useQuery({
    queryKey: ['plans'],
    queryFn: async () => {
      const response = await fetch(`${API_URL}/tenant/tiers`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      
      if (!response.ok) throw new Error('Failed to fetch plans');
      return response.json();
    },
    staleTime: 60 * 60 * 1000, // Cache for 1 hour
  });

  const handleSubscribe = async (priceId) => {
    setLoading(true);
    
    try {
      // Call your backend API to create a checkout session
      //Change ngrok domain
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

  if (plansLoading) {
    return (
      <div className="App">
        <header>
          <h1>Choose Your Subscription Plan</h1>
        </header>
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p>Loading available plans...</p>
        </div>
      </div>
    );
  }

  if (plansError) {
    return (
      <div className="App">
        <header>
          <h1>Choose Your Subscription Plan</h1>
        </header>
        <div className="error-container">
          <p>There was an error loading the subscription plans. Please try again later.</p>
          <button onClick={() => window.location.reload()} className="retry-button">
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="App">
      <header>
        <h1>Choose Your Subscription Plan</h1>
      </header>
      
      <div className="pricing-container">
        <div className="pricing-grid">
          {plans && plans.map((plan) => (
            <div key={plan.id} className={`pricing-card`}>
              <h2>{plan.name}</h2>
              <div className="price">
                <span className="amount">${plan.price}</span>
                <span className="interval">/month</span>
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
                {loading ? 'Processing...' : 'Subscribe'}
              </button>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}