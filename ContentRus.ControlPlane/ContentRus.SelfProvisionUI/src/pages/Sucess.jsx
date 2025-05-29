import React, { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { API_URL } from '../components/ApiUrl';

export const Success = () => {
  const [searchParams] = useSearchParams();
  const planId = searchParams.get('planId');
  const token = localStorage.getItem('token');

  useEffect(() => {
    if (!planId) return;

    const updateTenant = async () => {
      try {
        await fetch(`${API_URL}/tenant/tier`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: planId,
        });

        await fetch(`${API_URL}/tenant/state`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: '2',
        });
      } catch (err) {
        console.error('Error updating tenant info:', err);
      }
    };

    updateTenant();
  }, [planId, token]);

  return (
    <div className="App">
      <h1>Subscription Successful!</h1>
      <p>Your plan has been updated. Thank you!</p>
    </div>
  );
}
