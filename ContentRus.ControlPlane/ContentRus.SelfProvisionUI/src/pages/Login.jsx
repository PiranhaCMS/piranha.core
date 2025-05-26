import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { API_URL } from '../components/ApiUrl';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isRegister, setIsRegister] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async () => {
        setErrorMessage('');

        const endpoint = isRegister ? 'register' : 'login';
        const navigate_url = isRegister ? '/profile' : '/';

        try {
            const response = await fetch(`${API_URL}/user/${endpoint}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                const { token, username } = await response.json();
                localStorage.setItem('token', token);
                localStorage.setItem('username', username);
                navigate(navigate_url);
            } else {
                if (isRegister) {
                    setErrorMessage('Registration failed. Please try again.');
                } else {
                    setErrorMessage('Invalid credentials. Please try again.');
                }
            }
        } catch (err) {
            setErrorMessage('An error occurred. Please try again later.');
            console.error('Login error:', err);
        }
    };

    return (
        <div className="flex items-center justify-center min-h-screen bg-neutral-50 dark:bg-neutral-950 px-4">
            <div className="w-full max-w-md p-6 rounded-2xl border border-neutral-200 dark:border-neutral-800 shadow-lg">
                <h2 className="text-2xl font-semibold text-center mb-6 text-neutral-900 dark:text-neutral-100">
                    {isRegister ? 'Sign Up' : 'Login'}
                </h2>

                {errorMessage && (
                    <div className="mb-4 p-3 bg-red-100 dark:bg-red-900/20 border border-red-300 dark:border-red-800 text-red-700 dark:text-red-400 rounded-lg flex items-center">
                        <svg className="w-5 h-5 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
                        </svg>
                        <span>{errorMessage}</span>
                    </div>
                )}

                <input
                    type="email"
                    placeholder="Email"
                    className="w-full p-3 mb-4 border border-neutral-300 dark:border-neutral-700 rounded-lg bg-transparent text-neutral-900 dark:text-neutral-100 placeholder-neutral-500 dark:placeholder-neutral-400"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                />

                <input
                    type="password"
                    placeholder="Password"
                    className="w-full p-3 mb-4 border border-neutral-300 dark:border-neutral-700 rounded-lg bg-transparent text-neutral-900 dark:text-neutral-100 placeholder-neutral-500 dark:placeholder-neutral-400"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <button
                    onClick={handleSubmit}
                    className="w-full bg-neutral-800 dark:bg-neutral-200 text-neutral-900 dark:text-neutral-100 p-3 rounded-lg hover:opacity-90 transition"
                >
                    GO
                </button>

                <p className="text-center mt-4 text-sm text-neutral-700 dark:text-neutral-300">
                    {isRegister ? 'Already have an account?' : "Don't have an account yet?"}
                    <button
                        onClick={() => setIsRegister(!isRegister)}
                        className="text-blue-600 dark:text-blue-400 hover:underline"
                        style={{ marginLeft: '10px' }}
                    >
                        {isRegister ? 'Login' : 'Sign Up'}
                    </button>
                </p>
            </div>
        </div>
    );
}
