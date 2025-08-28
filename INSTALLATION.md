# دليل التثبيت - Bank Management System

## 📋 المتطلبات الأساسية

### متطلبات النظام
- **نظام التشغيل**: Windows 10/11, macOS 10.15+, Ubuntu 18.04+
- **الذاكرة**: 4GB RAM كحد أدنى، 8GB مُوصى به
- **مساحة القرص**: 2GB مساحة فارغة
- **الشبكة**: اتصال إنترنت لتحميل التبعيات

### البرامج المطلوبة
- **Docker Desktop** 4.0+ ([تحميل](https://www.docker.com/products/docker-desktop))
- **Docker Compose** 2.0+ (مُضمن مع Docker Desktop)
- **Git** ([تحميل](https://git-scm.com/downloads))

## 🚀 التثبيت السريع (Docker)

### الخطوة 1: تحميل المشروع
```bash
# استنساخ المشروع
git clone <repository-url>
cd bank-management-system

# أو فك ضغط الملف المُحمل
unzip bank-management-system.zip
cd bank-management-system
```

### الخطوة 2: تشغيل النظام
```bash
# تشغيل جميع الخدمات
docker-compose up -d

# مراقبة السجلات
docker-compose logs -f
```

### الخطوة 3: التحقق من التشغيل
```bash
# التحقق من حالة الخدمات
docker-compose ps

# يجب أن تظهر جميع الخدمات بحالة "Up"
```

### الخطوة 4: الوصول للنظام
- **الواجهة الأمامية**: http://localhost:3000
- **API الخلفية**: http://localhost:5000
- **توثيق API**: http://localhost:5000/swagger
- **قاعدة البيانات**: localhost:1433

## 🛠 التثبيت للتطوير

### متطلبات إضافية للتطوير
- **.NET 8 SDK** ([تحميل](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Node.js 18+** ([تحميل](https://nodejs.org/))
- **Visual Studio Code** أو **Visual Studio 2022**

### إعداد Backend (.NET)
```bash
cd backend/BankManagement.API

# استعادة الحزم
dotnet restore

# بناء المشروع
dotnet build

# تشغيل المشروع
dotnet run
```

### إعداد Frontend (React)
```bash
cd frontend/web

# تثبيت التبعيات
npm install
# أو
pnpm install

# تشغيل خادم التطوير
npm run dev
# أو
pnpm dev
```

### إعداد قاعدة البيانات
```bash
# تشغيل SQL Server فقط
docker-compose up sqlserver -d

# تطبيق المايجريشن
cd backend/BankManagement.API
dotnet ef database update
```

## 🔧 التكوين المتقدم

### تخصيص المنافذ
قم بتعديل ملف `docker-compose.yml`:
```yaml
services:
  frontend:
    ports:
      - "8080:80"  # تغيير المنفذ الأمامي
  backend:
    ports:
      - "8000:80"  # تغيير منفذ API
```

### تخصيص قاعدة البيانات
قم بتعديل متغيرات البيئة في `docker-compose.yml`:
```yaml
sqlserver:
  environment:
    - SA_PASSWORD=YourStrongPassword123!
    - MSSQL_PID=Express
```

### إعدادات الأمان
```bash
# إنشاء شهادات SSL
mkdir -p nginx/ssl
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx/ssl/nginx.key \
  -out nginx/ssl/nginx.crt
```

## 🐛 حل المشاكل الشائعة

### مشكلة: فشل في تشغيل SQL Server
```bash
# التحقق من السجلات
docker-compose logs sqlserver

# إعادة تشغيل الخدمة
docker-compose restart sqlserver

# التحقق من المساحة المتاحة
df -h
```

### مشكلة: فشل في الاتصال بقاعدة البيانات
```bash
# التحقق من حالة الشبكة
docker network ls
docker network inspect bank-management-system_bank-network

# اختبار الاتصال
docker-compose exec backend ping sqlserver
```

### مشكلة: بطء في تحميل الواجهة الأمامية
```bash
# إعادة بناء الصورة
docker-compose build frontend --no-cache

# التحقق من استخدام الموارد
docker stats
```

### مشكلة: خطأ في الأذونات
```bash
# إصلاح أذونات الملفات (Linux/macOS)
sudo chown -R $USER:$USER .
chmod -R 755 .

# Windows: تشغيل PowerShell كمدير
```

## 📊 مراقبة الأداء

### مراقبة استخدام الموارد
```bash
# مراقبة الحاويات
docker stats

# مراقبة السجلات
docker-compose logs -f --tail=100

# مراقبة قاعدة البيانات
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P BankSystem@2024 \
  -Q "SELECT name FROM sys.databases"
```

### فحص صحة النظام
```bash
# فحص صحة الخدمات
curl http://localhost:5000/health
curl http://localhost:3000/health

# فحص API
curl http://localhost:5000/api/accounts
```

## 🔄 التحديث والصيانة

### تحديث النظام
```bash
# إيقاف الخدمات
docker-compose down

# تحديث الكود
git pull origin main

# إعادة بناء الصور
docker-compose build --no-cache

# تشغيل النظام المحدث
docker-compose up -d
```

### النسخ الاحتياطي
```bash
# نسخ احتياطي لقاعدة البيانات
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P BankSystem@2024 \
  -Q "BACKUP DATABASE BankManagementDB TO DISK = '/var/opt/mssql/backup/bank_backup.bak'"

# نسخ الملف الاحتياطي
docker cp bank-sqlserver:/var/opt/mssql/backup/bank_backup.bak ./backup/
```

### استعادة النسخة الاحتياطية
```bash
# نسخ الملف الاحتياطي
docker cp ./backup/bank_backup.bak bank-sqlserver:/var/opt/mssql/backup/

# استعادة قاعدة البيانات
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P BankSystem@2024 \
  -Q "RESTORE DATABASE BankManagementDB FROM DISK = '/var/opt/mssql/backup/bank_backup.bak' WITH REPLACE"
```

## 🌐 النشر في الإنتاج

### متطلبات الإنتاج
- **خادم**: 4 CPU cores, 8GB RAM, 50GB SSD
- **نظام التشغيل**: Ubuntu 20.04 LTS أو CentOS 8
- **Docker**: 20.10+
- **شهادة SSL**: من Let's Encrypt أو مزود معتمد

### خطوات النشر
```bash
# إعداد متغيرات الإنتاج
cp .env.example .env.production
nano .env.production

# النشر
docker-compose -f docker-compose.prod.yml up -d

# إعداد SSL
certbot --nginx -d yourdomain.com
```

## 📞 الحصول على المساعدة

إذا واجهت أي مشاكل:

1. **تحقق من السجلات**: `docker-compose logs`
2. **راجع الوثائق**: README.md
3. **ابحث في المشاكل المعروفة**: GitHub Issues
4. **اتصل بالدعم**: support@bankmanagement.com

## ✅ قائمة التحقق بعد التثبيت

- [ ] جميع الحاويات تعمل بشكل صحيح
- [ ] يمكن الوصول للواجهة الأمامية
- [ ] API يستجيب بشكل صحيح
- [ ] قاعدة البيانات متصلة
- [ ] البيانات التجريبية محملة
- [ ] السجلات تعمل بشكل صحيح
- [ ] النسخ الاحتياطي مُعد

---

**تهانينا! 🎉 تم تثبيت نظام إدارة البنك بنجاح**

