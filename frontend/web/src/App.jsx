import { useState, useEffect } from 'react'
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { Toaster } from '@/components/ui/toaster'
import Navbar from '@/components/Navbar'
import Sidebar from '@/components/Sidebar'
import Dashboard from '@/components/Dashboard'
import Accounts from '@/components/Accounts'
import Transactions from '@/components/Transactions'
import Statistics from '@/components/Statistics'
import { ThemeProvider } from '@/components/ThemeProvider'
import './App.css'

function App() {
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const [currentPage, setCurrentPage] = useState('dashboard')

  return (
    <ThemeProvider defaultTheme="light" storageKey="bank-ui-theme">
      <Router>
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800">
          <Navbar 
            sidebarOpen={sidebarOpen} 
            setSidebarOpen={setSidebarOpen}
            currentPage={currentPage}
          />
          
          <div className="flex">
            <Sidebar 
              open={sidebarOpen} 
              setOpen={setSidebarOpen}
              currentPage={currentPage}
              setCurrentPage={setCurrentPage}
            />
            
            <main className="flex-1 p-4 lg:p-6 transition-all duration-300 ease-in-out">
              <div className="max-w-7xl mx-auto">
                <Routes>
                  <Route path="/" element={<Navigate to="/dashboard" replace />} />
                  <Route 
                    path="/dashboard" 
                    element={<Dashboard setCurrentPage={setCurrentPage} />} 
                  />
                  <Route 
                    path="/accounts" 
                    element={<Accounts setCurrentPage={setCurrentPage} />} 
                  />
                  <Route 
                    path="/transactions" 
                    element={<Transactions setCurrentPage={setCurrentPage} />} 
                  />
                  <Route 
                    path="/statistics" 
                    element={<Statistics setCurrentPage={setCurrentPage} />} 
                  />
                </Routes>
              </div>
            </main>
          </div>
          
          <Toaster />
        </div>
      </Router>
    </ThemeProvider>
  )
}

export default App

