import { useState, useEffect } from 'react'
import { 
  Users, 
  CreditCard, 
  TrendingUp, 
  DollarSign,
  ArrowUpRight,
  ArrowDownRight,
  Activity,
  Clock
} from 'lucide-react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Progress } from '@/components/ui/progress'
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell
} from 'recharts'
import { motion } from 'framer-motion'

const Dashboard = ({ setCurrentPage }) => {
  const [stats, setStats] = useState({
    totalAccounts: 0,
    activeAccounts: 0,
    totalBalance: 0,
    todayTransactions: 0,
    loading: true
  })

  const [recentTransactions, setRecentTransactions] = useState([])
  const [chartData, setChartData] = useState([])

  useEffect(() => {
    setCurrentPage('dashboard')
    fetchDashboardData()
  }, [setCurrentPage])

  const fetchDashboardData = async () => {
    try {
      // Simulate API calls
      setTimeout(() => {
        setStats({
          totalAccounts: 1247,
          activeAccounts: 1198,
          totalBalance: 15750000,
          todayTransactions: 89,
          loading: false
        })

        setRecentTransactions([
          {
            id: 1,
            type: 'deposit',
            amount: 5000,
            account: 'ACC123456789',
            owner: 'أحمد محمد علي',
            time: '10:30 ص'
          },
          {
            id: 2,
            type: 'withdrawal',
            amount: 1500,
            account: 'ACC987654321',
            owner: 'فاطمة أحمد السالم',
            time: '09:45 ص'
          },
          {
            id: 3,
            type: 'transfer',
            amount: 3000,
            account: 'ACC555666777',
            owner: 'شركة التقنية المتقدمة',
            time: '09:15 ص'
          }
        ])

        setChartData([
          { name: 'يناير', deposits: 4000, withdrawals: 2400, transfers: 2400 },
          { name: 'فبراير', deposits: 3000, withdrawals: 1398, transfers: 2210 },
          { name: 'مارس', deposits: 2000, withdrawals: 9800, transfers: 2290 },
          { name: 'أبريل', deposits: 2780, withdrawals: 3908, transfers: 2000 },
          { name: 'مايو', deposits: 1890, withdrawals: 4800, transfers: 2181 },
          { name: 'يونيو', deposits: 2390, withdrawals: 3800, transfers: 2500 }
        ])
      }, 1000)
    } catch (error) {
      console.error('Error fetching dashboard data:', error)
      setStats(prev => ({ ...prev, loading: false }))
    }
  }

  const statCards = [
    {
      title: 'إجمالي الحسابات',
      value: stats.totalAccounts.toLocaleString(),
      change: '+12%',
      changeType: 'positive',
      icon: Users,
      color: 'bg-blue-500'
    },
    {
      title: 'الحسابات النشطة',
      value: stats.activeAccounts.toLocaleString(),
      change: '+8%',
      changeType: 'positive',
      icon: Activity,
      color: 'bg-green-500'
    },
    {
      title: 'إجمالي الأرصدة',
      value: `${(stats.totalBalance / 1000000).toFixed(1)}م ريال`,
      change: '+15%',
      changeType: 'positive',
      icon: DollarSign,
      color: 'bg-purple-500'
    },
    {
      title: 'معاملات اليوم',
      value: stats.todayTransactions.toLocaleString(),
      change: '+23%',
      changeType: 'positive',
      icon: CreditCard,
      color: 'bg-orange-500'
    }
  ]

  const pieData = [
    { name: 'حسابات التوفير', value: 45, color: '#3B82F6' },
    { name: 'حسابات جارية', value: 35, color: '#10B981' },
    { name: 'حسابات تجارية', value: 20, color: '#F59E0B' }
  ]

  const getTransactionIcon = (type) => {
    switch (type) {
      case 'deposit':
        return <ArrowDownRight className="h-4 w-4 text-green-600" />
      case 'withdrawal':
        return <ArrowUpRight className="h-4 w-4 text-red-600" />
      case 'transfer':
        return <TrendingUp className="h-4 w-4 text-blue-600" />
      default:
        return <Activity className="h-4 w-4 text-gray-600" />
    }
  }

  const getTransactionColor = (type) => {
    switch (type) {
      case 'deposit':
        return 'text-green-600'
      case 'withdrawal':
        return 'text-red-600'
      case 'transfer':
        return 'text-blue-600'
      default:
        return 'text-gray-600'
    }
  }

  const getTransactionLabel = (type) => {
    switch (type) {
      case 'deposit':
        return 'إيداع'
      case 'withdrawal':
        return 'سحب'
      case 'transfer':
        return 'تحويل'
      default:
        return 'معاملة'
    }
  }

  if (stats.loading) {
    return (
      <div className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[1, 2, 3, 4].map((i) => (
            <Card key={i} className="animate-pulse">
              <CardContent className="p-6">
                <div className="h-4 bg-gray-200 rounded w-3/4 mb-4"></div>
                <div className="h-8 bg-gray-200 rounded w-1/2"></div>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Welcome Section */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
      >
        <Card className="bg-gradient-to-r from-blue-600 to-indigo-600 text-white">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-2xl font-bold mb-2">مرحباً بك في نظام إدارة البنك</h2>
                <p className="text-blue-100">
                  تابع أداء البنك وإدارة الحسابات والمعاملات من مكان واحد
                </p>
              </div>
              <div className="hidden md:block">
                <Clock className="h-16 w-16 text-blue-200" />
              </div>
            </div>
          </CardContent>
        </Card>
      </motion.div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {statCards.map((stat, index) => {
          const Icon = stat.icon
          return (
            <motion.div
              key={stat.title}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
            >
              <Card className="hover:shadow-lg transition-shadow duration-300">
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400">
                        {stat.title}
                      </p>
                      <p className="text-2xl font-bold text-gray-900 dark:text-white">
                        {stat.value}
                      </p>
                      <div className="flex items-center mt-2">
                        <Badge 
                          variant={stat.changeType === 'positive' ? 'default' : 'destructive'}
                          className="text-xs"
                        >
                          {stat.change}
                        </Badge>
                        <span className="text-xs text-gray-500 mr-2">من الشهر الماضي</span>
                      </div>
                    </div>
                    <div className={`p-3 rounded-full ${stat.color}`}>
                      <Icon className="h-6 w-6 text-white" />
                    </div>
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          )
        })}
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Line Chart */}
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5, delay: 0.4 }}
        >
          <Card>
            <CardHeader>
              <CardTitle>نشاط المعاملات الشهري</CardTitle>
              <CardDescription>
                مقارنة الإيداعات والسحوبات والتحويلات
              </CardDescription>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <LineChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="name" />
                  <YAxis />
                  <Tooltip />
                  <Line 
                    type="monotone" 
                    dataKey="deposits" 
                    stroke="#10B981" 
                    strokeWidth={2}
                    name="إيداعات"
                  />
                  <Line 
                    type="monotone" 
                    dataKey="withdrawals" 
                    stroke="#EF4444" 
                    strokeWidth={2}
                    name="سحوبات"
                  />
                  <Line 
                    type="monotone" 
                    dataKey="transfers" 
                    stroke="#3B82F6" 
                    strokeWidth={2}
                    name="تحويلات"
                  />
                </LineChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>
        </motion.div>

        {/* Pie Chart */}
        <motion.div
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5, delay: 0.5 }}
        >
          <Card>
            <CardHeader>
              <CardTitle>توزيع أنواع الحسابات</CardTitle>
              <CardDescription>
                النسبة المئوية لكل نوع حساب
              </CardDescription>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                  >
                    {pieData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>
        </motion.div>
      </div>

      {/* Recent Transactions */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.6 }}
      >
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <div>
              <CardTitle>المعاملات الأخيرة</CardTitle>
              <CardDescription>
                آخر المعاملات المنجزة اليوم
              </CardDescription>
            </div>
            <Button variant="outline" size="sm">
              عرض الكل
            </Button>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {recentTransactions.map((transaction) => (
                <div 
                  key={transaction.id}
                  className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg"
                >
                  <div className="flex items-center space-x-4 rtl:space-x-reverse">
                    <div className="p-2 bg-white dark:bg-gray-700 rounded-full">
                      {getTransactionIcon(transaction.type)}
                    </div>
                    <div>
                      <p className="font-medium text-gray-900 dark:text-white">
                        {getTransactionLabel(transaction.type)}
                      </p>
                      <p className="text-sm text-gray-500 dark:text-gray-400">
                        {transaction.owner} • {transaction.account}
                      </p>
                    </div>
                  </div>
                  <div className="text-left">
                    <p className={`font-bold ${getTransactionColor(transaction.type)}`}>
                      {transaction.amount.toLocaleString()} ريال
                    </p>
                    <p className="text-sm text-gray-500 dark:text-gray-400">
                      {transaction.time}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  )
}

export default Dashboard

