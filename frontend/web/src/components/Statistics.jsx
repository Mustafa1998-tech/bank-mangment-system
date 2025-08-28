import { useState, useEffect } from 'react'
import { 
  TrendingUp, 
  TrendingDown,
  Users,
  CreditCard,
  DollarSign,
  Calendar,
  Download,
  RefreshCw,
  BarChart3,
  PieChart,
  Activity
} from 'lucide-react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs'
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
  PieChart as RechartsPieChart,
  Pie,
  Cell,
  AreaChart,
  Area
} from 'recharts'
import { motion } from 'framer-motion'

const Statistics = ({ setCurrentPage }) => {
  const [loading, setLoading] = useState(true)
  const [timeRange, setTimeRange] = useState('month')
  const [statistics, setStatistics] = useState({})
  const [chartData, setChartData] = useState({})

  useEffect(() => {
    setCurrentPage('statistics')
    fetchStatistics()
  }, [setCurrentPage, timeRange])

  const fetchStatistics = async () => {
    setLoading(true)
    try {
      // Simulate API call
      setTimeout(() => {
        setStatistics({
          totalAccounts: 1247,
          activeAccounts: 1198,
          totalBalance: 15750000,
          totalTransactions: 8945,
          totalDeposits: 12500000,
          totalWithdrawals: 8750000,
          totalTransfers: 5200000,
          averageBalance: 12634,
          newAccountsThisMonth: 45,
          transactionsToday: 89,
          topAccountType: 'Savings',
          growthRate: 12.5
        })

        setChartData({
          monthlyTransactions: [
            { name: 'يناير', deposits: 4000, withdrawals: 2400, transfers: 2400, total: 8800 },
            { name: 'فبراير', deposits: 3000, withdrawals: 1398, transfers: 2210, total: 6608 },
            { name: 'مارس', deposits: 2000, withdrawals: 9800, transfers: 2290, total: 14090 },
            { name: 'أبريل', deposits: 2780, withdrawals: 3908, transfers: 2000, total: 8688 },
            { name: 'مايو', deposits: 1890, withdrawals: 4800, transfers: 2181, total: 8871 },
            { name: 'يونيو', deposits: 2390, withdrawals: 3800, transfers: 2500, total: 8690 }
          ],
          accountTypes: [
            { name: 'حسابات التوفير', value: 45, count: 560, color: '#3B82F6' },
            { name: 'حسابات جارية', value: 35, count: 437, color: '#10B981' },
            { name: 'حسابات تجارية', value: 20, count: 250, color: '#F59E0B' }
          ],
          dailyActivity: [
            { time: '00:00', transactions: 12 },
            { time: '04:00', transactions: 8 },
            { time: '08:00', transactions: 45 },
            { time: '12:00', transactions: 78 },
            { time: '16:00', transactions: 65 },
            { time: '20:00', transactions: 34 }
          ],
          balanceDistribution: [
            { range: '0-1000', count: 234, percentage: 18.8 },
            { range: '1000-5000', count: 456, percentage: 36.6 },
            { range: '5000-10000', count: 312, percentage: 25.0 },
            { range: '10000-50000', count: 189, percentage: 15.2 },
            { range: '50000+', count: 56, percentage: 4.4 }
          ]
        })
        setLoading(false)
      }, 1000)
    } catch (error) {
      console.error('Error fetching statistics:', error)
      setLoading(false)
    }
  }

  const kpiCards = [
    {
      title: 'إجمالي الحسابات',
      value: statistics.totalAccounts?.toLocaleString() || '0',
      change: '+12%',
      changeType: 'positive',
      icon: Users,
      color: 'bg-blue-500'
    },
    {
      title: 'إجمالي الأرصدة',
      value: `${(statistics.totalBalance / 1000000)?.toFixed(1) || '0'}م ريال`,
      change: '+15%',
      changeType: 'positive',
      icon: DollarSign,
      color: 'bg-green-500'
    },
    {
      title: 'إجمالي المعاملات',
      value: statistics.totalTransactions?.toLocaleString() || '0',
      change: '+23%',
      changeType: 'positive',
      icon: CreditCard,
      color: 'bg-purple-500'
    },
    {
      title: 'متوسط الرصيد',
      value: `${statistics.averageBalance?.toLocaleString() || '0'} ريال`,
      change: '+8%',
      changeType: 'positive',
      icon: TrendingUp,
      color: 'bg-orange-500'
    }
  ]

  const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444', '#8B5CF6']

  if (loading) {
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
      {/* Header */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4"
      >
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
            الإحصائيات والتقارير
          </h1>
          <p className="text-gray-600 dark:text-gray-400">
            تحليل شامل لأداء البنك والمعاملات المالية
          </p>
        </div>

        <div className="flex gap-2">
          <Select value={timeRange} onValueChange={setTimeRange}>
            <SelectTrigger className="w-[180px]">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="week">الأسبوع الماضي</SelectItem>
              <SelectItem value="month">الشهر الماضي</SelectItem>
              <SelectItem value="quarter">الربع الماضي</SelectItem>
              <SelectItem value="year">السنة الماضية</SelectItem>
            </SelectContent>
          </Select>
          <Button variant="outline" size="sm">
            <Download className="h-4 w-4 ml-2" />
            تصدير التقرير
          </Button>
          <Button variant="outline" size="sm" onClick={fetchStatistics}>
            <RefreshCw className="h-4 w-4 ml-2" />
            تحديث
          </Button>
        </div>
      </motion.div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {kpiCards.map((kpi, index) => {
          const Icon = kpi.icon
          return (
            <motion.div
              key={kpi.title}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.5, delay: index * 0.1 }}
            >
              <Card className="hover:shadow-lg transition-shadow duration-300">
                <CardContent className="p-6">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-gray-600 dark:text-gray-400">
                        {kpi.title}
                      </p>
                      <p className="text-2xl font-bold text-gray-900 dark:text-white">
                        {kpi.value}
                      </p>
                      <div className="flex items-center mt-2">
                        <Badge 
                          variant={kpi.changeType === 'positive' ? 'default' : 'destructive'}
                          className="text-xs"
                        >
                          {kpi.change}
                        </Badge>
                        <span className="text-xs text-gray-500 mr-2">من الفترة السابقة</span>
                      </div>
                    </div>
                    <div className={`p-3 rounded-full ${kpi.color}`}>
                      <Icon className="h-6 w-6 text-white" />
                    </div>
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          )
        })}
      </div>

      {/* Charts Tabs */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.4 }}
      >
        <Tabs defaultValue="transactions" className="space-y-4">
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="transactions">المعاملات</TabsTrigger>
            <TabsTrigger value="accounts">الحسابات</TabsTrigger>
            <TabsTrigger value="activity">النشاط</TabsTrigger>
            <TabsTrigger value="distribution">التوزيع</TabsTrigger>
          </TabsList>

          <TabsContent value="transactions" className="space-y-4">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Monthly Transactions Line Chart */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-5 w-5" />
                    نشاط المعاملات الشهري
                  </CardTitle>
                  <CardDescription>
                    مقارنة الإيداعات والسحوبات والتحويلات
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <LineChart data={chartData.monthlyTransactions}>
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

              {/* Transaction Volume Area Chart */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Activity className="h-5 w-5" />
                    حجم المعاملات الإجمالي
                  </CardTitle>
                  <CardDescription>
                    إجمالي المعاملات الشهرية
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <AreaChart data={chartData.monthlyTransactions}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Area 
                        type="monotone" 
                        dataKey="total" 
                        stroke="#8B5CF6" 
                        fill="#8B5CF6"
                        fillOpacity={0.3}
                        name="إجمالي المعاملات"
                      />
                    </AreaChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>
            </div>
          </TabsContent>

          <TabsContent value="accounts" className="space-y-4">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Account Types Pie Chart */}
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <PieChart className="h-5 w-5" />
                    توزيع أنواع الحسابات
                  </CardTitle>
                  <CardDescription>
                    النسبة المئوية لكل نوع حساب
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <RechartsPieChart>
                      <Pie
                        data={chartData.accountTypes}
                        cx="50%"
                        cy="50%"
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                        label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                      >
                        {chartData.accountTypes?.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={entry.color} />
                        ))}
                      </Pie>
                      <Tooltip />
                    </RechartsPieChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>

              {/* Account Types Bar Chart */}
              <Card>
                <CardHeader>
                  <CardTitle>عدد الحسابات حسب النوع</CardTitle>
                  <CardDescription>
                    العدد الفعلي للحسابات
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={chartData.accountTypes}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="count" fill="#3B82F6" />
                    </BarChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>
            </div>
          </TabsContent>

          <TabsContent value="activity" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Activity className="h-5 w-5" />
                  النشاط اليومي للمعاملات
                </CardTitle>
                <CardDescription>
                  توزيع المعاملات على مدار اليوم
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ResponsiveContainer width="100%" height={400}>
                  <AreaChart data={chartData.dailyActivity}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="time" />
                    <YAxis />
                    <Tooltip />
                    <Area 
                      type="monotone" 
                      dataKey="transactions" 
                      stroke="#10B981" 
                      fill="#10B981"
                      fillOpacity={0.3}
                      name="عدد المعاملات"
                    />
                  </AreaChart>
                </ResponsiveContainer>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="distribution" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>توزيع الأرصدة</CardTitle>
                <CardDescription>
                  توزيع العملاء حسب فئات الأرصدة
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {chartData.balanceDistribution?.map((item, index) => (
                    <div key={item.range} className="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
                      <div className="flex items-center gap-3">
                        <div 
                          className="w-4 h-4 rounded-full"
                          style={{ backgroundColor: COLORS[index % COLORS.length] }}
                        />
                        <span className="font-medium">{item.range} ريال</span>
                      </div>
                      <div className="flex items-center gap-4">
                        <span className="text-sm text-gray-600 dark:text-gray-400">
                          {item.count} عميل
                        </span>
                        <Badge variant="outline">
                          {item.percentage}%
                        </Badge>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </motion.div>

      {/* Summary Cards */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.6 }}
        className="grid grid-cols-1 md:grid-cols-3 gap-6"
      >
        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">ملخص الإيداعات</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600 mb-2">
              {(statistics.totalDeposits / 1000000)?.toFixed(1)}م ريال
            </div>
            <p className="text-sm text-gray-600 dark:text-gray-400">
              إجمالي الإيداعات هذا الشهر
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">ملخص السحوبات</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-red-600 mb-2">
              {(statistics.totalWithdrawals / 1000000)?.toFixed(1)}م ريال
            </div>
            <p className="text-sm text-gray-600 dark:text-gray-400">
              إجمالي السحوبات هذا الشهر
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-3">
            <CardTitle className="text-lg">ملخص التحويلات</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600 mb-2">
              {(statistics.totalTransfers / 1000000)?.toFixed(1)}م ريال
            </div>
            <p className="text-sm text-gray-600 dark:text-gray-400">
              إجمالي التحويلات هذا الشهر
            </p>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  )
}

export default Statistics

