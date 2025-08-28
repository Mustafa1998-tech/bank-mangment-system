import { useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { 
  LayoutDashboard, 
  Users, 
  CreditCard, 
  TrendingUp, 
  Settings,
  X,
  ChevronRight
} from 'lucide-react'
import { Button } from '@/components/ui/button'
import { cn } from '@/lib/utils'

const Sidebar = ({ open, setOpen, currentPage, setCurrentPage }) => {
  const location = useLocation()

  const menuItems = [
    {
      id: 'dashboard',
      label: 'لوحة التحكم',
      icon: LayoutDashboard,
      path: '/dashboard',
      color: 'text-blue-600'
    },
    {
      id: 'accounts',
      label: 'الحسابات',
      icon: Users,
      path: '/accounts',
      color: 'text-green-600'
    },
    {
      id: 'transactions',
      label: 'المعاملات',
      icon: CreditCard,
      path: '/transactions',
      color: 'text-purple-600'
    },
    {
      id: 'statistics',
      label: 'الإحصائيات',
      icon: TrendingUp,
      path: '/statistics',
      color: 'text-orange-600'
    }
  ]

  const handleItemClick = (item) => {
    setCurrentPage(item.id)
    setOpen(false) // Close sidebar on mobile after selection
  }

  return (
    <>
      {/* Overlay for mobile */}
      {open && (
        <div 
          className="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
          onClick={() => setOpen(false)}
        />
      )}

      {/* Sidebar */}
      <div className={cn(
        "fixed inset-y-0 left-0 z-50 w-64 bg-white dark:bg-gray-800 shadow-xl transform transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static lg:inset-0",
        open ? "translate-x-0" : "-translate-x-full"
      )}>
        <div className="flex flex-col h-full">
          {/* Header */}
          <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700 lg:hidden">
            <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
              القائمة
            </h2>
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setOpen(false)}
            >
              <X className="h-5 w-5" />
            </Button>
          </div>

          {/* Navigation */}
          <nav className="flex-1 px-4 py-6 space-y-2">
            {menuItems.map((item) => {
              const Icon = item.icon
              const isActive = location.pathname === item.path
              
              return (
                <Link
                  key={item.id}
                  to={item.path}
                  onClick={() => handleItemClick(item)}
                  className={cn(
                    "flex items-center px-4 py-3 text-sm font-medium rounded-lg transition-all duration-200 group",
                    isActive
                      ? "bg-blue-50 dark:bg-blue-900/20 text-blue-700 dark:text-blue-300 shadow-sm"
                      : "text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 hover:text-gray-900 dark:hover:text-white"
                  )}
                >
                  <Icon className={cn(
                    "h-5 w-5 ml-3",
                    isActive ? item.color : "text-gray-400 group-hover:text-gray-500"
                  )} />
                  <span className="flex-1">{item.label}</span>
                  {isActive && (
                    <ChevronRight className="h-4 w-4 text-blue-600" />
                  )}
                </Link>
              )
            })}
          </nav>

          {/* Footer */}
          <div className="p-4 border-t border-gray-200 dark:border-gray-700">
            <Link
              to="/settings"
              className="flex items-center px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-200"
            >
              <Settings className="h-5 w-5 ml-3 text-gray-400" />
              <span>الإعدادات</span>
            </Link>
            
            <div className="mt-4 p-3 bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 rounded-lg">
              <p className="text-xs text-gray-600 dark:text-gray-400 text-center">
                نظام إدارة البنك v2.0
              </p>
              <p className="text-xs text-gray-500 dark:text-gray-500 text-center mt-1">
                جميع الحقوق محفوظة © 2024
              </p>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}

export default Sidebar

