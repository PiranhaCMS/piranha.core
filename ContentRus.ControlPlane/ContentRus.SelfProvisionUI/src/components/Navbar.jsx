import React, { useState, useRef, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './../styles/navbar.css'

export function Navbar() {
  const [isDropdownOpen, setDropdownOpen] = useState(false);
  const [username, setUsername] = useState('User');
  const dropdownRef = useRef(null);
  const navigate = useNavigate();

  useEffect(() => {
    const storedUsername = localStorage.getItem('username');
    if (storedUsername) {
      setUsername(storedUsername);
    }
  }, []);

  useEffect(() => {
    function handleClickOutside(event) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false);
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  return (
    <nav className="fixed top-0 w-full bg-gray-800 text-white py-2 px-4 shadow z-50 flex justify-between items-center">
      <ul className="flex gap-4">
        <li>
          <Link to="/" className="hover:underline">Main</Link>
        </li>
        <li>
          <Link to="/test" className="hover:underline">Test</Link>
        </li>
      </ul>

      <div className="relative" ref={dropdownRef}>
        <button
          onClick={() => setDropdownOpen(!isDropdownOpen)}
          className="flex items-center gap-1 bg-gray-700 px-2 py-1 rounded-lg hover:bg-gray-600 transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-opacity-50"
        >
          <div className="w-6 h-6 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">
            {username.charAt(0)}
          </div>
          <span className="text-sm text-gray-900 dark:text-white">{username}</span>
          <svg 
            className={`w-3 h-3 ml-1 transition-transform duration-200 ${isDropdownOpen ? 'rotate-180' : ''} text-gray-900 dark:text-white`} 
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
          </svg>
        </button>

        {isDropdownOpen && (
          <div className="absolute right-0 mt-1 w-56 origin-top-right bg-white dark:bg-gray-800 text-gray-800 dark:text-white rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 overflow-hidden transition-all duration-200 transform animate-dropdown">
            <div className="py-1">
              <Link
                to="/profile"
                className="flex items-center px-4 py-2.5 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-150"
                onClick={() => setDropdownOpen(false)}
              >
                <svg className="w-5 h-5 mr-3 text-gray-500 dark:text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                <span className="text-gray-500 dark:text-gray-400">Profile</span>
              </Link>
              
              <Link
                to="/billing"
                className="flex items-center px-4 py-2.5 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-150"
                onClick={() => setDropdownOpen(false)}
              >
                <svg className="w-5 h-5 mr-3 text-gray-500 dark:text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                </svg>
                <span className="text-gray-500 dark:text-gray-400">Billing</span>
              </Link>
            </div>
            
            <div className="py-1 border-t border-gray-200 dark:border-gray-700">
              <Link
                to="/login"
                onClick={(e) => {
                  e.preventDefault();
                  localStorage.removeItem('token');
                  navigate('/login');
                  setDropdownOpen(false);
                }}
                className="flex items-center px-4 py-2.5 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors duration-150"
              >
                <svg className="w-5 h-5 mr-3 text-red-500 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
                <span className="text-red-500 dark:text-red-400">Sign Out</span>
              </Link>
            </div>
          </div>
        )}
      </div>
    </nav>
  );
}
