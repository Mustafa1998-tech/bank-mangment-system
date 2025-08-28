# نظام إدارة البنك - Bank Management System

نظام شامل لإدارة البنك مطور باستخدام .NET 8، SQL Server، و React مع TypeScript. يوفر النظام واجهة حديثة ومتجاوبة لإدارة الحسابات المصرفية والمعاملات المالية.

## 🚀 الميزات الرئيسية

### 📊 لوحة التحكم
- إحصائيات شاملة في الوقت الفعلي
- مخططات تفاعلية للمعاملات والأرصدة
- ملخص سريع للأنشطة اليومية
- مؤشرات الأداء الرئيسية (KPIs)

### 👥 إدارة الحسابات
- إنشاء وتعديل الحسابات المصرفية
- دعم أنواع مختلفة من الحسابات (توفير، جاري، تجاري)
- البحث والفلترة المتقدمة
- إدارة حالات الحسابات (نشط، معلق، مغلق)
- تتبع تاريخ المعاملات لكل حساب

### 💳 إدارة المعاملات
- إيداع وسحب الأموال
- تحويلات بين الحسابات
- حساب الرسوم تلقائياً
- تتبع حالة المعاملات
- سجل شامل لجميع العمليات

### 📈 التقارير والإحصائيات
- تقارير مالية تفصيلية
- مخططات بيانية متنوعة
- إحصائيات الأداء الشهرية والسنوية
- تحليل أنماط المعاملات
- تصدير التقارير

### 🎨 واجهة المستخدم
- تصميم حديث ومتجاوب
- دعم الوضع الليلي والنهاري
- واجهة باللغة العربية
- تجربة مستخدم محسنة
- رسوم متحركة سلسة

## 🛠 التقنيات المستخدمة

### Backend (.NET 8)
- **ASP.NET Core 8** - إطار العمل الرئيسي
- **Entity Framework Core** - ORM لقاعدة البيانات
- **SQL Server** - قاعدة البيانات
- **AutoMapper** - تحويل البيانات
- **Serilog** - نظام السجلات
- **Swagger/OpenAPI** - توثيق API

### Frontend (React)
- **React 18** - مكتبة واجهة المستخدم
- **Vite** - أداة البناء
- **Tailwind CSS** - إطار عمل CSS
- **Shadcn/ui** - مكونات واجهة المستخدم
- **Recharts** - مخططات بيانية
- **React Router** - التنقل
- **Framer Motion** - الرسوم المتحركة

### DevOps & Deployment
- **Docker & Docker Compose** - الحاويات
- **Nginx** - خادم الويب والبروكسي العكسي
- **Multi-stage builds** - تحسين الحاويات

## 📋 متطلبات النظام

- **Docker** 20.10+
- **Docker Compose** 2.0+
- **Node.js** 18+ (للتطوير)
- **.NET 8 SDK** (للتطوير)

## 🚀 التشغيل السريع

### باستخدام Docker Compose (الطريقة المفضلة)

```bash
# استنساخ المشروع
git clone <repository-url>
cd bank-management-system

# تشغيل جميع الخدمات
docker-compose up -d

# انتظار تشغيل الخدمات (حوالي 2-3 دقائق)
docker-compose logs -f

# الوصول للتطبيق
# Frontend: http://localhost:3000
# Backend API: http://localhost:5000
# API Documentation: http://localhost:5000/swagger
```

### التطوير المحلي

#### Backend
```bash
cd backend/BankManagement.API
dotnet restore
dotnet run
```

#### Frontend
```bash
cd frontend/web
npm install
npm run dev
```

## 🗄 قاعدة البيانات

يستخدم النظام SQL Server مع Entity Framework Core. يتم إنشاء قاعدة البيانات تلقائياً عند التشغيل الأول مع بيانات تجريبية.

### الجداول الرئيسية:
- **Accounts** - الحسابات المصرفية
- **Transactions** - المعاملات المالية
- **Cards** - البطاقات المصرفية
- **Loans** - القروض
- **LoanPayments** - دفعات القروض

## 🔧 التكوين

### متغيرات البيئة

#### Backend
```env
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=BankManagementDB;User Id=sa;Password=BankSystem@2024;TrustServerCertificate=true
ASPNETCORE_ENVIRONMENT=Production
```

#### Frontend
```env
REACT_APP_API_URL=http://localhost:5000
REACT_APP_ENVIRONMENT=production
```

## 📚 API Documentation

يوفر النظام توثيق شامل لـ API باستخدام Swagger:
- **URL**: http://localhost:5000/swagger
- **Format**: OpenAPI 3.0

### نقاط النهاية الرئيسية:

#### الحسابات
- `GET /api/accounts` - قائمة الحسابات
- `POST /api/accounts` - إنشاء حساب جديد
- `GET /api/accounts/{id}` - تفاصيل حساب
- `PUT /api/accounts/{id}` - تحديث حساب
- `DELETE /api/accounts/{id}` - حذف حساب

#### المعاملات
- `POST /api/transactions/accounts/{id}/deposit` - إيداع
- `POST /api/transactions/accounts/{id}/withdraw` - سحب
- `POST /api/transactions/accounts/{id}/transfer` - تحويل
- `GET /api/transactions/{id}` - تفاصيل معاملة

## 🧪 الاختبار

### اختبار الوحدة
```bash
cd backend/BankManagement.Tests
dotnet test
```

### اختبار التكامل
```bash
docker-compose -f docker-compose.test.yml up --abort-on-container-exit
```

## 🔒 الأمان

- **HTTPS** - تشفير الاتصالات
- **CORS** - حماية من الطلبات المتقاطعة
- **Input Validation** - التحقق من صحة البيانات
- **SQL Injection Protection** - حماية من حقن SQL
- **Error Handling** - معالجة الأخطاء الآمنة

## 📊 المراقبة والسجلات

- **Serilog** - نظام سجلات متقدم
- **Health Checks** - فحص صحة الخدمات
- **Application Insights** - مراقبة الأداء
- **Structured Logging** - سجلات منظمة

## 🚀 النشر

### الإنتاج
```bash
# بناء الصور
docker-compose -f docker-compose.prod.yml build

# النشر
docker-compose -f docker-compose.prod.yml up -d
```

### البيئة السحابية
- دعم **Azure Container Instances**
- دعم **AWS ECS**
- دعم **Google Cloud Run**

## 🤝 المساهمة

1. Fork المشروع
2. إنشاء فرع للميزة (`git checkout -b feature/AmazingFeature`)
3. Commit التغييرات (`git commit -m 'Add some AmazingFeature'`)
4. Push للفرع (`git push origin feature/AmazingFeature`)
5. فتح Pull Request

## 📝 الترخيص

هذا المشروع مرخص تحت رخصة MIT - راجع ملف [LICENSE](LICENSE) للتفاصيل.

## 👨‍💻 المطورون

- **فريق التطوير** - التطوير الأولي

## 🙏 شكر وتقدير

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [React](https://reactjs.org/)
- [Tailwind CSS](https://tailwindcss.com/)
- [Shadcn/ui](https://ui.shadcn.com/)

## 📞 الدعم

للحصول على الدعم، يرجى فتح issue في GitHub أو التواصل عبر البريد الإلكتروني.

---

**ملاحظة**: هذا المشروع مطور لأغراض تعليمية وتجريبية. للاستخدام في الإنتاج، يرجى مراجعة متطلبات الأمان والامتثال المصرفي.

