import React, { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { API_URL } from '../components/ApiUrl';

const updateTenantTier = async ({ token, planId }) => {
  return fetch(`${API_URL}/tenant/tier`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: planId,
  });
};

const updateTenantState = async (token) => {
  return fetch(`${API_URL}/tenant/state`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(2), // use JSON.stringify for safety
  });
};

export const Success = () => {
  const [searchParams] = useSearchParams();
  const planId = searchParams.get('planId');
  const token = localStorage.getItem('token');

  const tierMutation = useMutation({
    mutationFn: updateTenantTier,
    onError: (error) => {
      console.error('Error updating tenant tier:', error);
    },
  });

  const stateMutation = useMutation({
    mutationFn: updateTenantState,
    onError: (error) => {
      console.error('Error updating tenant state:', error);
    },
  });

  useEffect(() => {
    if (!planId || !token) return;

    tierMutation.mutate({ token, planId }, {
      onSuccess: () => {
        stateMutation.mutate(token);
      }
    });
  }, [planId, token]);

  return (
    <div className="App">
      <h1>Subscription Successful!</h1>
      <p>Your plan has been updated. Thank you!</p>
    </div>
  );
};
