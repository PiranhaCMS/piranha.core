import './App.css'

import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Navbar } from './components/Navbar';
import { Test } from './pages/Test';
import { Main } from './pages/Main';
import {Billing} from './pages/Billing';


function App() {

  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/" element={<Main />} />
        <Route path="/test" element={<Test />} />
        <Route path="/bill" element={< Billing />} />
      </Routes>
    </Router>
  )
}

export default App
