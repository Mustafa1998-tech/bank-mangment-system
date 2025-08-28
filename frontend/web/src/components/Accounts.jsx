import { useState, useEffect } from 'react'
import { 
  Plus, 
  Search, 
  Filter, 
  MoreHorizontal,
  Eye,
  Edit,
  Trash2,
  UserCheck,
  UserX,
  DollarSign
} from 'lucide-react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { motion } from 'framer-motion'
import { useToast } from '@/components/ui/use-toast'

const Accounts = ({ setCurrentPage }) => {
  const [accounts, setAccounts] = useState([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [filterType, setFilterType] = useState('all')
  const [filterStatus, setFilterStatus] = useState('all')
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const [selectedAccount, setSelectedAccount] = useState(null)
  const { toast } = useToast()

  const [newAccount, setNewAccount] = useState({
    ownerName: '',
    email: '',
    phoneNumber: '',
    accountType: 'Savings',
    initialBalance: 0,
    notes: ''
  })

  useEffect(() => {
    setCurrentPage('accounts')
    fetchAccounts()
  }, [setCurrentPage])

  const fetchAccounts = async () => {
    try {
      // Simulate API call
      setTimeout(() => {
        setAccounts([
          {
            id: 1,
            accountNumber: 'ACC123456789',
            ownerName: 'أحمد محمد علي',
            email: 'ahmed.ali@example.com',
            phoneNumber: '+966501234567',
            balance: 15750.00,
            accountType: 'Savings',
            status: 'Active',
            createdAt: '2024-01-15',
            transactionCount: 45,
            cardCount: 2,
            loanCount: 0
          },
          {
            id: 2,
            accountNumber: 'ACC987654321',
            ownerName: 'فاطمة أحمد السالم',
            email: 'fatima.salem@example.com',
            phoneNumber: '+966507654321',
            balance: 28900.00,
            accountType: 'Checking',
            status: 'Active',
            createdAt: '2024-02-20',
            transactionCount: 78,
            cardCount: 1,
            loanCount: 1
          },
          {
            id: 3,
            accountNumber: 'ACC555666777',
            ownerName: 'شركة التقنية المتقدمة',
            email: 'info@techadvanced.com',
            phoneNumber: '+966112345678',
            balance: 125000.00,
            accountType: 'Business',
            status: 'Active',
            createdAt: '2023-12-10',
            transactionCount: 156,
            cardCount: 3,
            loanCount: 2
          },
          {
            id: 4,
            accountNumber: 'ACC111222333',
            ownerName: 'محمد عبدالله الحربي',
            email: 'mohammed.harbi@example.com',
            phoneNumber: '+966509876543',
            balance: 5200.00,
            accountType: 'Savings',
            status: 'Suspended',
            createdAt: '2024-03-05',
            transactionCount: 12,
            cardCount: 1,
            loanCount: 0
          }
        ])
        setLoading(false)
      }, 1000)
    } catch (error) {
      console.error('Error fetching accounts:', error)
      setLoading(false)
    }
  }

  const handleCreateAccount = async () => {
    try {
      // Simulate API call
      const accountData = {
        ...newAccount,
        id: accounts.length + 1,
        accountNumber: `ACC${Math.random().toString().substr(2, 9)}`,
        balance: newAccount.initialBalance,
        status: 'Active',
        createdAt: new Date().toISOString().split('T')[0],
        transactionCount: 0,
        cardCount: 0,
        loanCount: 0
      }

      setAccounts([...accounts, accountData])
      setIsCreateDialogOpen(false)
      setNewAccount({
        ownerName: '',
        email: '',
        phoneNumber: '',
        accountType: 'Savings',
        initialBalance: 0,
        notes: ''
      })

      toast({
        title: "تم إنشاء الحساب بنجاح",
        description: `تم إنشاء حساب جديد برقم ${accountData.accountNumber}`,
      })
    } catch (error) {
      toast({
        title: "خطأ في إنشاء الحساب",
        description: "حدث خطأ أثناء إنشاء الحساب. يرجى المحاولة مرة أخرى.",
        variant: "destructive",
      })
    }
  }

  const handleStatusChange = async (accountId, newStatus) => {
    try {
      setAccounts(accounts.map(account => 
        account.id === accountId 
          ? { ...account, status: newStatus }
          : account
      ))

      toast({
        title: "تم تحديث حالة الحساب",
        description: `تم تغيير حالة الحساب إلى ${newStatus}`,
      })
    } catch (error) {
      toast({
        title: "خطأ في تحديث الحساب",
        description: "حدث خطأ أثناء تحديث حالة الحساب.",
        variant: "destructive",
      })
    }
  }

  const filteredAccounts = accounts.filter(account => {
    const matchesSearch = account.ownerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         account.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         account.accountNumber.includes(searchTerm)
    
    const matchesType = filterType === 'all' || account.accountType === filterType
    const matchesStatus = filterStatus === 'all' || account.status === filterStatus

    return matchesSearch && matchesType && matchesStatus
  })

  const getStatusBadge = (status) => {
    const variants = {
      Active: 'default',
      Suspended: 'secondary',
      Closed: 'destructive'
    }
    
    const labels = {
      Active: 'نشط',
      Suspended: 'معلق',
      Closed: 'مغلق'
    }

    return (
      <Badge variant={variants[status] || 'default'}>
        {labels[status] || status}
      </Badge>
    )
  }

  const getAccountTypeLabel = (type) => {
    const labels = {
      Savings: 'توفير',
      Checking: 'جاري',
      Business: 'تجاري'
    }
    return labels[type] || type
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <Card className="animate-pulse">
          <CardContent className="p-6">
            <div className="h-8 bg-gray-200 rounded w-1/4 mb-4"></div>
            <div className="space-y-3">
              {[1, 2, 3, 4, 5].map((i) => (
                <div key={i} className="h-4 bg-gray-200 rounded"></div>
              ))}
            </div>
          </CardContent>
        </Card>
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
            إدارة الحسابات
          </h1>
          <p className="text-gray-600 dark:text-gray-400">
            إدارة حسابات العملاء ومتابعة أنشطتهم المصرفية
          </p>
        </div>

        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button className="bg-blue-600 hover:bg-blue-700">
              <Plus className="h-4 w-4 ml-2" />
              إنشاء حساب جديد
            </Button>
          </DialogTrigger>
          <DialogContent className="sm:max-w-[425px]">
            <DialogHeader>
              <DialogTitle>إنشاء حساب جديد</DialogTitle>
              <DialogDescription>
                أدخل بيانات العميل لإنشاء حساب مصرفي جديد
              </DialogDescription>
            </DialogHeader>
            <div className="grid gap-4 py-4">
              <div className="grid gap-2">
                <Label htmlFor="ownerName">اسم صاحب الحساب</Label>
                <Input
                  id="ownerName"
                  value={newAccount.ownerName}
                  onChange={(e) => setNewAccount({...newAccount, ownerName: e.target.value})}
                  placeholder="أدخل الاسم الكامل"
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="email">البريد الإلكتروني</Label>
                <Input
                  id="email"
                  type="email"
                  value={newAccount.email}
                  onChange={(e) => setNewAccount({...newAccount, email: e.target.value})}
                  placeholder="example@email.com"
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="phoneNumber">رقم الهاتف</Label>
                <Input
                  id="phoneNumber"
                  value={newAccount.phoneNumber}
                  onChange={(e) => setNewAccount({...newAccount, phoneNumber: e.target.value})}
                  placeholder="+966xxxxxxxxx"
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="accountType">نوع الحساب</Label>
                <Select 
                  value={newAccount.accountType} 
                  onValueChange={(value) => setNewAccount({...newAccount, accountType: value})}
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Savings">حساب توفير</SelectItem>
                    <SelectItem value="Checking">حساب جاري</SelectItem>
                    <SelectItem value="Business">حساب تجاري</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="grid gap-2">
                <Label htmlFor="initialBalance">الرصيد الأولي</Label>
                <Input
                  id="initialBalance"
                  type="number"
                  value={newAccount.initialBalance}
                  onChange={(e) => setNewAccount({...newAccount, initialBalance: parseFloat(e.target.value) || 0})}
                  placeholder="0.00"
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="notes">ملاحظات</Label>
                <Textarea
                  id="notes"
                  value={newAccount.notes}
                  onChange={(e) => setNewAccount({...newAccount, notes: e.target.value})}
                  placeholder="ملاحظات إضافية (اختياري)"
                />
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" onClick={() => setIsCreateDialogOpen(false)}>
                إلغاء
              </Button>
              <Button onClick={handleCreateAccount}>
                إنشاء الحساب
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </motion.div>

      {/* Filters */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.1 }}
      >
        <Card>
          <CardContent className="p-6">
            <div className="flex flex-col sm:flex-row gap-4">
              <div className="flex-1">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                  <Input
                    placeholder="البحث بالاسم، البريد الإلكتروني، أو رقم الحساب..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>
              <Select value={filterType} onValueChange={setFilterType}>
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="نوع الحساب" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">جميع الأنواع</SelectItem>
                  <SelectItem value="Savings">حساب توفير</SelectItem>
                  <SelectItem value="Checking">حساب جاري</SelectItem>
                  <SelectItem value="Business">حساب تجاري</SelectItem>
                </SelectContent>
              </Select>
              <Select value={filterStatus} onValueChange={setFilterStatus}>
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="الحالة" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">جميع الحالات</SelectItem>
                  <SelectItem value="Active">نشط</SelectItem>
                  <SelectItem value="Suspended">معلق</SelectItem>
                  <SelectItem value="Closed">مغلق</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </CardContent>
        </Card>
      </motion.div>

      {/* Accounts Table */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.2 }}
      >
        <Card>
          <CardHeader>
            <CardTitle>قائمة الحسابات ({filteredAccounts.length})</CardTitle>
            <CardDescription>
              جميع الحسابات المصرفية المسجلة في النظام
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>صاحب الحساب</TableHead>
                    <TableHead>رقم الحساب</TableHead>
                    <TableHead>النوع</TableHead>
                    <TableHead>الرصيد</TableHead>
                    <TableHead>الحالة</TableHead>
                    <TableHead>المعاملات</TableHead>
                    <TableHead>تاريخ الإنشاء</TableHead>
                    <TableHead className="text-center">الإجراءات</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredAccounts.map((account) => (
                    <TableRow key={account.id}>
                      <TableCell>
                        <div>
                          <div className="font-medium">{account.ownerName}</div>
                          <div className="text-sm text-gray-500">{account.email}</div>
                        </div>
                      </TableCell>
                      <TableCell className="font-mono">{account.accountNumber}</TableCell>
                      <TableCell>{getAccountTypeLabel(account.accountType)}</TableCell>
                      <TableCell>
                        <div className="flex items-center">
                          <DollarSign className="h-4 w-4 text-green-600 ml-1" />
                          <span className="font-medium">
                            {account.balance.toLocaleString()} ريال
                          </span>
                        </div>
                      </TableCell>
                      <TableCell>{getStatusBadge(account.status)}</TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{account.transactionCount} معاملة</div>
                          <div className="text-gray-500">
                            {account.cardCount} بطاقة • {account.loanCount} قرض
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>{account.createdAt}</TableCell>
                      <TableCell>
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button variant="ghost" className="h-8 w-8 p-0">
                              <MoreHorizontal className="h-4 w-4" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <DropdownMenuItem>
                              <Eye className="mr-2 h-4 w-4" />
                              عرض التفاصيل
                            </DropdownMenuItem>
                            <DropdownMenuItem>
                              <Edit className="mr-2 h-4 w-4" />
                              تعديل البيانات
                            </DropdownMenuItem>
                            {account.status === 'Active' ? (
                              <DropdownMenuItem 
                                onClick={() => handleStatusChange(account.id, 'Suspended')}
                              >
                                <UserX className="mr-2 h-4 w-4" />
                                تعليق الحساب
                              </DropdownMenuItem>
                            ) : (
                              <DropdownMenuItem 
                                onClick={() => handleStatusChange(account.id, 'Active')}
                              >
                                <UserCheck className="mr-2 h-4 w-4" />
                                تنشيط الحساب
                              </DropdownMenuItem>
                            )}
                            <DropdownMenuItem className="text-red-600">
                              <Trash2 className="mr-2 h-4 w-4" />
                              حذف الحساب
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  )
}

export default Accounts

