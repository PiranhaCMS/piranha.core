import React from 'react';
import { Navigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { API_URL } from '../components/ApiUrl';

export function ProtectedRoute({ children }) {
  const token = localStorage.getItem('token');

  if (!token) {
    return <Navigate to="/login" replace />;
  }



  const { isLoading, isError } = useQuery({
    queryKey: ['validateToken'],
    queryFn: async () => {
      const response = await fetch(`${API_URL}/user/validate`, {
        headers: { Authorization: `Bearer ${token}` },
      });

      if (!response.ok) {
        throw new Error('Token validation failed');
      }
      const data = await response.json();
      return data;
    },
    retry: false,
    refetchOnWindowFocus: false,
  });

  if (isLoading) return (
    <div className="flex items-center justify-center min-h-[400px]">
      <div className="animate-pulse text-blue-600 dark:text-blue-400">
        <svg className="w-12 h-12 animate-spin" fill="none" viewBox="0 0 24 24">
          <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
          <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
      </div>
    </div>
  );
  if (isError) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
