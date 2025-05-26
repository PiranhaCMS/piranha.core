import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { API_URL } from '../components/ApiUrl';

export function Profile() {
    const navigate = useNavigate();
    const token = localStorage.getItem('token');
    const queryClient = useQueryClient();
    const [editMode, setEditMode] = useState(false);
    const [isFirstEdit, setIsFirstEdit] = useState(false);
    const [formData, setFormData] = useState({ name: '', address: '', country: '' });

    // Fetch countries from REST Countries API
    const { data: countriesData, isLoading: countriesLoading } = useQuery({
        queryKey: ['countries'],
        queryFn: async () => {
            const response = await fetch('https://restcountries.com/v3.1/all');
            if (!response.ok) throw new Error('Failed to fetch countries');
            const data = await response.json();
            
            return data.map(elem => ({ 
                country: elem.name.common, 
                flag: elem.flag 
            })).sort((a, b) => a.country.localeCompare(b.country));
        },
        staleTime: 24 * 60 * 60 * 1000, // Cache for 24 hours
    });

    const { data: tenant, isLoading, isError } = useQuery({
        queryKey: ['tenant'],
        queryFn: async () => {
            const res = await fetch(`${API_URL}/tenant`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error('Failed to fetch tenant');
            const json = await res.json();
            if (!json.name || !json.address || !json.country) {
                // console.log('Incomplete tenant data, setting edit mode');
                setEditMode(true);
                setIsFirstEdit(true);
            }
            setFormData({
                name: json.name || '',
                address: json.address || '',
                country: json.country || '',
            });
            return json;
        },
        /*
        onSuccess: (data) => {
            console.log('Tenant data fetched:', data);
            if (!data.name || !data.address || !data.country) {
                setEditMode(true); // Auto-edit if data is incomplete
            }
            setFormData({
                name: data.name || '',
                address: data.address || '',
                country: data.country || '',
            });
        },
        */
    });

    const updateTenantMutation = useMutation({
        mutationFn: async (formData) => {
            const res = await fetch(`${API_URL}/tenant/info`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(formData),
            });
            if (!res.ok) throw new Error('Failed to update tenant');
        },
        onSuccess: () => {
            queryClient.invalidateQueries(['tenant']);
            setEditMode(false);
            localStorage.setItem('username', formData.name);
            
            if (isFirstEdit) {
                navigate('/billing');
            }
        },
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        updateTenantMutation.mutate(formData);
    };

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

    if (isError) return (
        <div className="p-8 text-center">
            <div className="text-red-500 dark:text-red-400 text-xl mb-2">Error loading profile</div>
        </div>
    );

    return (
        <div className="max-w-3xl mx-auto p-6 animate-fadeIn">
            <div className="text-center mb-10">
                <div className="inline-block p-3 rounded-full bg-blue-100 dark:bg-blue-900/30 mb-4">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10 text-blue-600 dark:text-blue-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                    </svg>
                </div>
                <h2 className="text-3xl font-bold text-gray-800 dark:text-white mb-2">Company Profile</h2>
                <p className="text-gray-600 dark:text-gray-400">Manage your organization's information</p>
            </div>

            <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg border border-gray-200 dark:border-gray-700 overflow-hidden transition-all duration-300">
                {editMode ? (
                    <form onSubmit={handleSubmit} className="p-8">
                        <div className="space-y-8">
                            <div className="relative">
                                <label className="block mb-2 font-medium text-gray-700 dark:text-gray-300">Name <span className="text-red-500 dark:text-red-400">*</span></label>
                                <input
                                    name="name"
                                    value={formData.name}
                                    onChange={handleChange}
                                    className="w-full px-4 py-3 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent transition-all duration-200"
                                    required
                                    placeholder="Enter your company name"
                                />
                            </div>

                            <div className="relative">
                                <label className="block mb-2 font-medium text-gray-700 dark:text-gray-300">Address <span className="text-red-500 dark:text-red-400">*</span></label>
                                <input
                                    name="address"
                                    value={formData.address}
                                    onChange={handleChange}
                                    className="w-full px-4 py-3 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent transition-all duration-200"
                                    required
                                    placeholder="Enter your company address"
                                />
                            </div>

                            <div className="relative">
                                <label className="block mb-2 font-medium text-gray-700 dark:text-gray-300">Country <span className="text-red-500 dark:text-red-400">*</span></label>
                                <select
                                    name="country"
                                    value={formData.country}
                                    onChange={handleChange}
                                    className="w-full px-4 py-3 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent transition-all duration-200"
                                    required
                                >
                                    <option value="">Select a country</option>
                                    {countriesLoading ? (
                                        <option value="" disabled>Loading countries...</option>
                                    ) : countriesData ? (
                                        countriesData.map((item) => (
                                            <option key={item.country} value={item.country}>
                                                {item.country} {item.flag}
                                            </option>
                                        ))
                                    ) : (
                                        <option value="" disabled>No countries available</option>
                                    )}
                                    
                                </select>
                            </div>

                            <div className="flex justify-end gap-3 pt-4">
                                {!isFirstEdit && (
                                    <button
                                        type="button"
                                        onClick={() => setEditMode(false)}
                                        className="px-5 py-2.5 rounded-lg border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 font-medium hover:bg-gray-50 dark:hover:bg-gray-700 transition-all duration-200"
                                    >
                                        Cancel
                                    </button>
                                )}
                                <button
                                    type="submit"
                                    disabled={updateTenantMutation.isPending}
                                    className="px-5 py-2.5 rounded-lg bg-blue-600 dark:bg-blue-700 text-white font-medium hover:bg-blue-700 dark:hover:bg-blue-600 transition-all duration-200 flex items-center"
                                >
                                    {updateTenantMutation.isPending ? (
                                        <>
                                            <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
                                                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                            </svg>
                                            Saving...
                                        </>
                                    ) : 'Save'}
                                </button>
                            </div>
                        </div>
                    </form>
                ) : (
                    <div className="p-8">
                        <div className="space-y-6">
                            <div className="border-b border-gray-100 dark:border-gray-700 pb-4">
                                <p className="text-sm font-semibold text-gray-500 dark:text-gray-400 mb-1">Name</p>
                                <p className="text-lg text-gray-800 dark:text-gray-200">{formData.name}</p>
                            </div>
                            <div className="border-b border-gray-100 dark:border-gray-700 pb-4">
                                <p className="text-sm font-semibold text-gray-500 dark:text-gray-400 mb-1">Address</p>
                                <p className="text-lg text-gray-800 dark:text-gray-200">{formData.address}</p>
                            </div>
                            <div className="border-b border-gray-100 dark:border-gray-700 pb-4">
                                <p className="text-sm font-semibold text-gray-500 dark:text-gray-400 mb-1">Country</p>
                                <p className="text-lg text-gray-800 dark:text-gray-200">
                                    {countriesData && formData.country ? (
                                        <>
                                            {formData.country} {countriesData.find(c => c.country === formData.country)?.flag || ''}
                                        </>
                                    ) : formData.country}
                                </p>
                            </div>
                            <div className="mt-8 text-right">
                                <button
                                    onClick={() => setEditMode(true)
                                    }
                                    className="px-5 py-2.5 rounded-lg bg-blue-600 dark:bg-blue-700 text-white font-medium hover:bg-blue-700 dark:hover:bg-blue-600 transition-all duration-200"
                                >
                                    Edit
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

