import { useState, useEffect } from 'react'
import { 
  Plus, 
  Search, 
  ArrowUpRight,
  ArrowDownRight,
  TrendingUp,
  Calendar,
  Filter,
  Download,
  RefreshCw
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
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '@/components/ui/tabs'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import { motion } from 'framer-motion'
import { useToast } from '@/components/ui/use-toast'

const Transactions = ({ setCurrentPage }) => {
  const [transactions, setTransactions] = useState([])
  const [accounts, setAccounts] = useState([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [filterType, setFilterType] = useState('all')
  const [filterStatus, setFilterStatus] = useState('all')
  const [isTransactionDialogOpen, setIsTransactionDialogOpen] = useState(false)
  const [activeTab, setActiveTab] = useState('deposit')
  const { toast } = useToast()

  const [transactionForm, setTransactionForm] = useState({
    accountId: '',
    amount: '',
    description: '',
    reference: '',
    toAccountNumber: ''
  })

  useEffect(() => {
    setCurrentPage('transactions')
    fetchTransactions()
    fetchAccounts()
  }, [setCurrentPage])

  const fetchTransactions = async () => {
    try {
      // Simulate API call
      setTimeout(() => {
        setTransactions([
          {
            id: 1,
            transactionId: 'TXN001234567890',
            accountId: 1,
            accountNumber: 'ACC123456789',
            accountOwnerName: 'أحمد محمد علي',
            transactionType: 'Deposit',
            amount: 5000.00,
            balanceAfter: 20750.00,
            description: 'إيداع نقدي',
            status: 'Completed',
            fee: 0,
            timestamp: '2024-01-20T10:30:00Z'
          },
          {
            id: 2,
            transactionId: 'TXN001234567891',
            accountId: 2,
            accountNumber: 'ACC987654321',
            accountOwnerName: 'فاطمة أحمد السالم',
            transactionType: 'Withdrawal',
            amount: 1500.00,
            balanceAfter: 27400.00,
            description: 'سحب نقدي من الصراف',
            status: 'Completed',
            fee: 5.00,
            timestamp: '2024-01-20T09:45:00Z'
          },
          {
            id: 3,
            transactionId: 'TXN001234567892',
            accountId: 1,
            accountNumber: 'ACC123456789',
            accountOwnerName: 'أحمد محمد علي',
            transactionType: 'Transfer',
            amount: 3000.00,
            balanceAfter: 17750.00,
            description: 'تحويل إلى ACC987654321',
            recipientAccount: 'ACC987654321',
            recipientName: 'فاطمة أحمد السالم',
            status: 'Completed',
            fee: 2.00,
            timestamp: '2024-01-20T08:15:00Z'
          },
          {
            id: 4,
            transactionId: 'TXN001234567893',
            accountId: 3,
            accountNumber: 'ACC555666777',
            accountOwnerName: 'شركة التقنية المتقدمة',
            transactionType: 'Deposit',
            amount: 25000.00,
            balanceAfter: 150000.00,
            description: 'إيداع شيك',
            status: 'Pending',
            fee: 0,
            timestamp: '2024-01-20T07:30:00Z'
          }
        ])
        setLoading(false)
      }, 1000)
    } catch (error) {
      console.error('Error fetching transactions:', error)
      setLoading(false)
    }
  }

  const fetchAccounts = async () => {
    try {
      // Simulate API call
      setAccounts([
        { id: 1, accountNumber: 'ACC123456789', ownerName: 'أحمد محمد علي', balance: 15750.00 },
        { id: 2, accountNumber: 'ACC987654321', ownerName: 'فاطمة أحمد السالم', balance: 28900.00 },
        { id: 3, accountNumber: 'ACC555666777', ownerName: 'شركة التقنية المتقدمة', balance: 125000.00 }
      ])
    } catch (error) {
      console.error('Error fetching accounts:', error)
    }
  }

  const handleTransaction = async (type) => {
    try {
      const selectedAccount = accounts.find(acc => acc.id === parseInt(transactionForm.accountId))
      if (!selectedAccount) {
        toast({
          title: "خطأ في البيانات",
          description: "يرجى اختيار حساب صحيح",
          variant: "destructive",
        })
        return
      }

      const amount = parseFloat(transactionForm.amount)
      if (amount <= 0) {
        toast({
          title: "خطأ في المبلغ",
          description: "يرجى إدخال مبلغ صحيح",
          variant: "destructive",
        })
        return
      }

      // Simulate API call
      const newTransaction = {
        id: transactions.length + 1,
        transactionId: `TXN${Math.random().toString().substr(2, 12)}`,
        accountId: selectedAccount.id,
        accountNumber: selectedAccount.accountNumber,
        accountOwnerName: selectedAccount.ownerName,
        transactionType: type.charAt(0).toUpperCase() + type.slice(1),
        amount: amount,
        balanceAfter: type === 'deposit' ? selectedAccount.balance + amount : selectedAccount.balance - amount,
        description: transactionForm.description || `${getTransactionLabel(type)} ${type === 'transfer' ? `إلى ${transactionForm.toAccountNumber}` : ''}`,
        recipientAccount: type === 'transfer' ? transactionForm.toAccountNumber : null,
        status: 'Completed',
        fee: type === 'withdrawal' ? 5.00 : type === 'transfer' ? 2.00 : 0,
        timestamp: new Date().toISOString()
      }

      setTransactions([newTransaction, ...transactions])
      setIsTransactionDialogOpen(false)
      setTransactionForm({
        accountId: '',
        amount: '',
        description: '',
        reference: '',
        toAccountNumber: ''
      })

      toast({
        title: "تمت المعاملة بنجاح",
        description: `تم ${getTransactionLabel(type)} مبلغ ${amount.toLocaleString()} ريال`,
      })
    } catch (error) {
      toast({
        title: "خطأ في المعاملة",
        description: "حدث خطأ أثناء تنفيذ المعاملة. يرجى المحاولة مرة أخرى.",
        variant: "destructive",
      })
    }
  }

  const getTransactionIcon = (type) => {
    switch (type.toLowerCase()) {
      case 'deposit':
        return <ArrowDownRight className="h-4 w-4 text-green-600" />
      case 'withdrawal':
        return <ArrowUpRight className="h-4 w-4 text-red-600" />
      case 'transfer':
        return <TrendingUp className="h-4 w-4 text-blue-600" />
      default:
        return <TrendingUp className="h-4 w-4 text-gray-600" />
    }
  }

  const getTransactionColor = (type) => {
    switch (type.toLowerCase()) {
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
    switch (type.toLowerCase()) {
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

  const getStatusBadge = (status) => {
    const variants = {
      Completed: 'default',
      Pending: 'secondary',
      Failed: 'destructive',
      Cancelled: 'outline'
    }
    
    const labels = {
      Completed: 'مكتملة',
      Pending: 'معلقة',
      Failed: 'فاشلة',
      Cancelled: 'ملغية'
    }

    return (
      <Badge variant={variants[status] || 'default'}>
        {labels[status] || status}
      </Badge>
    )
  }

  const formatDateTime = (dateString) => {
    const date = new Date(dateString)
    return {
      date: date.toLocaleDateString('ar-SA'),
      time: date.toLocaleTimeString('ar-SA', { hour: '2-digit', minute: '2-digit' })
    }
  }

  const filteredTransactions = transactions.filter(transaction => {
    const matchesSearch = transaction.accountOwnerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         transaction.accountNumber.includes(searchTerm) ||
                         transaction.transactionId.includes(searchTerm) ||
                         transaction.description.toLowerCase().includes(searchTerm.toLowerCase())
    
    const matchesType = filterType === 'all' || transaction.transactionType.toLowerCase() === filterType.toLowerCase()
    const matchesStatus = filterStatus === 'all' || transaction.status === filterStatus

    return matchesSearch && matchesType && matchesStatus
  })

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
            إدارة المعاملات
          </h1>
          <p className="text-gray-600 dark:text-gray-400">
            تنفيذ ومتابعة العمليات المصرفية المختلفة
          </p>
        </div>

        <div className="flex gap-2">
          <Button variant="outline" size="sm">
            <Download className="h-4 w-4 ml-2" />
            تصدير
          </Button>
          <Button variant="outline" size="sm" onClick={fetchTransactions}>
            <RefreshCw className="h-4 w-4 ml-2" />
            تحديث
          </Button>
          <Dialog open={isTransactionDialogOpen} onOpenChange={setIsTransactionDialogOpen}>
            <DialogTrigger asChild>
              <Button className="bg-blue-600 hover:bg-blue-700">
                <Plus className="h-4 w-4 ml-2" />
                معاملة جديدة
              </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[500px]">
              <DialogHeader>
                <DialogTitle>إجراء معاملة جديدة</DialogTitle>
                <DialogDescription>
                  اختر نوع المعاملة وأدخل البيانات المطلوبة
                </DialogDescription>
              </DialogHeader>
              
              <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
                <TabsList className="grid w-full grid-cols-3">
                  <TabsTrigger value="deposit">إيداع</TabsTrigger>
                  <TabsTrigger value="withdrawal">سحب</TabsTrigger>
                  <TabsTrigger value="transfer">تحويل</TabsTrigger>
                </TabsList>
                
                <TabsContent value="deposit" className="space-y-4">
                  <div className="grid gap-4">
                    <div className="grid gap-2">
                      <Label htmlFor="account">الحساب</Label>
                      <Select 
                        value={transactionForm.accountId} 
                        onValueChange={(value) => setTransactionForm({...transactionForm, accountId: value})}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="اختر الحساب" />
                        </SelectTrigger>
                        <SelectContent>
                          {accounts.map((account) => (
                            <SelectItem key={account.id} value={account.id.toString()}>
                              {account.ownerName} - {account.accountNumber}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="amount">المبلغ</Label>
                      <Input
                        id="amount"
                        type="number"
                        value={transactionForm.amount}
                        onChange={(e) => setTransactionForm({...transactionForm, amount: e.target.value})}
                        placeholder="0.00"
                      />
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="description">الوصف</Label>
                      <Textarea
                        id="description"
                        value={transactionForm.description}
                        onChange={(e) => setTransactionForm({...transactionForm, description: e.target.value})}
                        placeholder="وصف المعاملة (اختياري)"
                      />
                    </div>
                  </div>
                </TabsContent>
                
                <TabsContent value="withdrawal" className="space-y-4">
                  <div className="grid gap-4">
                    <div className="grid gap-2">
                      <Label htmlFor="account">الحساب</Label>
                      <Select 
                        value={transactionForm.accountId} 
                        onValueChange={(value) => setTransactionForm({...transactionForm, accountId: value})}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="اختر الحساب" />
                        </SelectTrigger>
                        <SelectContent>
                          {accounts.map((account) => (
                            <SelectItem key={account.id} value={account.id.toString()}>
                              {account.ownerName} - {account.accountNumber}
                              <span className="text-sm text-gray-500 mr-2">
                                (الرصيد: {account.balance.toLocaleString()} ريال)
                              </span>
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="amount">المبلغ</Label>
                      <Input
                        id="amount"
                        type="number"
                        value={transactionForm.amount}
                        onChange={(e) => setTransactionForm({...transactionForm, amount: e.target.value})}
                        placeholder="0.00"
                      />
                      <p className="text-xs text-gray-500">رسوم السحب: 5 ريال</p>
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="description">الوصف</Label>
                      <Textarea
                        id="description"
                        value={transactionForm.description}
                        onChange={(e) => setTransactionForm({...transactionForm, description: e.target.value})}
                        placeholder="وصف المعاملة (اختياري)"
                      />
                    </div>
                  </div>
                </TabsContent>
                
                <TabsContent value="transfer" className="space-y-4">
                  <div className="grid gap-4">
                    <div className="grid gap-2">
                      <Label htmlFor="account">الحساب المرسل</Label>
                      <Select 
                        value={transactionForm.accountId} 
                        onValueChange={(value) => setTransactionForm({...transactionForm, accountId: value})}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="اختر الحساب المرسل" />
                        </SelectTrigger>
                        <SelectContent>
                          {accounts.map((account) => (
                            <SelectItem key={account.id} value={account.id.toString()}>
                              {account.ownerName} - {account.accountNumber}
                              <span className="text-sm text-gray-500 mr-2">
                                (الرصيد: {account.balance.toLocaleString()} ريال)
                              </span>
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="toAccount">الحساب المستقبل</Label>
                      <Input
                        id="toAccount"
                        value={transactionForm.toAccountNumber}
                        onChange={(e) => setTransactionForm({...transactionForm, toAccountNumber: e.target.value})}
                        placeholder="رقم الحساب المستقبل"
                      />
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="amount">المبلغ</Label>
                      <Input
                        id="amount"
                        type="number"
                        value={transactionForm.amount}
                        onChange={(e) => setTransactionForm({...transactionForm, amount: e.target.value})}
                        placeholder="0.00"
                      />
                      <p className="text-xs text-gray-500">رسوم التحويل: 2 ريال</p>
                    </div>
                    <div className="grid gap-2">
                      <Label htmlFor="description">الوصف</Label>
                      <Textarea
                        id="description"
                        value={transactionForm.description}
                        onChange={(e) => setTransactionForm({...transactionForm, description: e.target.value})}
                        placeholder="وصف المعاملة (اختياري)"
                      />
                    </div>
                  </div>
                </TabsContent>
              </Tabs>
              
              <DialogFooter>
                <Button variant="outline" onClick={() => setIsTransactionDialogOpen(false)}>
                  إلغاء
                </Button>
                <Button onClick={() => handleTransaction(activeTab)}>
                  تنفيذ المعاملة
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </div>
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
                    placeholder="البحث برقم المعاملة، الحساب، أو الوصف..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
              </div>
              <Select value={filterType} onValueChange={setFilterType}>
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="نوع المعاملة" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">جميع الأنواع</SelectItem>
                  <SelectItem value="deposit">إيداع</SelectItem>
                  <SelectItem value="withdrawal">سحب</SelectItem>
                  <SelectItem value="transfer">تحويل</SelectItem>
                </SelectContent>
              </Select>
              <Select value={filterStatus} onValueChange={setFilterStatus}>
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="الحالة" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">جميع الحالات</SelectItem>
                  <SelectItem value="Completed">مكتملة</SelectItem>
                  <SelectItem value="Pending">معلقة</SelectItem>
                  <SelectItem value="Failed">فاشلة</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </CardContent>
        </Card>
      </motion.div>

      {/* Transactions Table */}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.2 }}
      >
        <Card>
          <CardHeader>
            <CardTitle>قائمة المعاملات ({filteredTransactions.length})</CardTitle>
            <CardDescription>
              جميع المعاملات المصرفية المسجلة في النظام
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>رقم المعاملة</TableHead>
                    <TableHead>النوع</TableHead>
                    <TableHead>الحساب</TableHead>
                    <TableHead>المبلغ</TableHead>
                    <TableHead>الرسوم</TableHead>
                    <TableHead>الحالة</TableHead>
                    <TableHead>التاريخ والوقت</TableHead>
                    <TableHead>الوصف</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredTransactions.map((transaction) => {
                    const { date, time } = formatDateTime(transaction.timestamp)
                    return (
                      <TableRow key={transaction.id}>
                        <TableCell className="font-mono text-sm">
                          {transaction.transactionId}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            {getTransactionIcon(transaction.transactionType)}
                            <span>{getTransactionLabel(transaction.transactionType)}</span>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div>
                            <div className="font-medium">{transaction.accountOwnerName}</div>
                            <div className="text-sm text-gray-500 font-mono">
                              {transaction.accountNumber}
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <span className={`font-bold ${getTransactionColor(transaction.transactionType)}`}>
                            {transaction.transactionType.toLowerCase() === 'withdrawal' ? '-' : '+'}
                            {transaction.amount.toLocaleString()} ريال
                          </span>
                        </TableCell>
                        <TableCell>
                          {transaction.fee > 0 ? (
                            <span className="text-red-600">
                              {transaction.fee.toLocaleString()} ريال
                            </span>
                          ) : (
                            <span className="text-gray-400">-</span>
                          )}
                        </TableCell>
                        <TableCell>{getStatusBadge(transaction.status)}</TableCell>
                        <TableCell>
                          <div className="text-sm">
                            <div>{date}</div>
                            <div className="text-gray-500">{time}</div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <div className="max-w-xs truncate" title={transaction.description}>
                            {transaction.description}
                          </div>
                          {transaction.recipientAccount && (
                            <div className="text-xs text-gray-500 mt-1">
                              إلى: {transaction.recipientAccount}
                            </div>
                          )}
                        </TableCell>
                      </TableRow>
                    )
                  })}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  )
}

export default Transactions

