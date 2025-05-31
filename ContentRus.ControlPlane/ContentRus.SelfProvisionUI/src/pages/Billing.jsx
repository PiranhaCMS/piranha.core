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


  // Fetch tenant info (tier and state)
  const { data: tenant, isLoading: tenantLoading, error: tenantError } = useQuery({
    queryKey: ['tenant'],
    queryFn: async () => {
      const response = await fetch(`${API_URL}/tenant`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (!response.ok) throw new Error('Failed to fetch tenant info');
      return response.json();
    },
  });

  const handleSubscribe = async (id, priceId) => {
    setLoading(true);
    const token = localStorage.getItem('token');
    
    try {
      // Call your backend API to create a checkout session
      //Change ngrok domain
      const response = await fetch('https://f004-193-137-169-167.ngrok-free.app/api/stripe/create-checkout-session', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          priceId: priceId,
          tenantId: token,
          id: id.toString(),
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

      try {
        await fetch(`${API_URL}/tenant/tier`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: id,
        });
      } catch (tierError) {
        console.error('Error updating tenant tier:', tierError);
      }

      // Update tenant state to ACTIVE after successful subscription
      try {
        await fetch(`${API_URL}/tenant/state`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
          },
          body: 2,
        });
      } catch (stateError) {
        console.error('Error updating tenant state:', stateError);
      }

      //window.location.reload();
      setLoading(false);
    }
  };

  if (plansLoading || tenantLoading) {
    return (
      <div className="App">
        <header>
          <h1>Choose Your Subscription Plan</h1>
        </header>
        <div className="loading-container">
          <div className="loading-spinner"></div>
          <p>Loading...</p>
        </div>
      </div>
    );
  }

  if (plansError || tenantError) {
    return (
      <div className="App">
        <header>
          <h1>Choose Your Subscription Plan</h1>
        </header>
        <div className="error-container">
          <p>There was an error loading data. Please try again later.</p>
          <button onClick={() => window.location.reload()} className="retry-button">Retry</button>
        </div>
      </div>
    );
  }

  let warningMessage = null;
  let displayActivePlan = false;
  if (tenant?.state === 1) {
    warningMessage = <p className="warning">Waiting for payment</p>;
  } else if (tenant?.state === 3) {
    warningMessage = <p className="warning cancelled">Cancelled</p>;
  } else {
    displayActivePlan = true;
  }

  return (
    <div className="App">
      <header>
        <h1>Choose Your Subscription Plan</h1>
        {warningMessage}
      </header>
      
      <div className="pricing-container">
        <div className="pricing-grid">
          {plans && plans.map((plan) => {
            const isActive = tenant?.tier === plan.id && displayActivePlan;

            return (
              <div
                key={plan.id}
                className={`pricing-card ${isActive ? 'active-plan' : ''}`}
              >
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
                  onClick={() => handleSubscribe(plan.id, plan.priceId)}
                  disabled={loading || isActive}
                >
                  {loading ? 'Processing...' : isActive ? 'Current Plan' : 'Subscribe'}
                </button>
              </div>
            );
          })}
        </div>
      </div>

    </div>
  );
}