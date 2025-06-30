# اسنپ‌داکتر - پلتفرم دیجیتال سلامت

## درباره پروژه

اسنپ‌داکتر یک پلتفرم مدرن و ریسپانسیو برای مشاوره‌های پزشکی و روانشناسی آنلاین است که با استفاده از تکنولوژی‌های روز دنیا طراحی شده است.

## ویژگی‌های اصلی

- 🏥 **مشاوره آنلاین پزشک**: مشاوره متنی و تلفنی فوری
- 🧠 **مشاوره روانشناسی**: جلسات روانشناسی آنلاین
- 💊 **نسخه الکترونیک**: دریافت نسخه دیجیتال
- 📱 **طراحی ریسپانسیو**: سازگار با موبایل و دسکتاپ
- 🔐 **احراز هویت امن**: سیستم OTP با کاوه‌نگار
- 🌐 **چندزبانه**: پشتیبانی کامل از زبان فارسی

## معماری پروژه

پروژه بر اساس **Clean Architecture** طراحی شده است:

```
src/
├── SnappDoctor.Core/           # لایه Domain (Entities, Enums)
├── SnappDoctor.Application/    # لایه Application (DTOs, Contracts)
├── SnappDoctor.Infrastructure/ # لایه Infrastructure (Data, Services)
└── SnappDoctor.Web/           # لایه Presentation (MVC)
```

## تکنولوژی‌های استفاده شده

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server + Entity Framework Core
- **Frontend**: HTML5, Tailwind CSS, JavaScript
- **Authentication**: ASP.NET Core Identity
- **SMS Service**: Kavenegar API
- **Validation**: FluentValidation
- **Fonts**: Vazirmatn (فونت فارسی)

## پیش‌نیازها

- .NET 8.0 SDK
- SQL Server (LocalDB یا SQL Server)
- Visual Studio 2022 یا VS Code

## نصب و راه‌اندازی

### 1. کلون کردن پروژه
```bash
git clone <repository-url>
cd Healthcare
```

### 2. تنظیم Connection String
فایل `appsettings.Development.json` را از `appsettings.Development.json.example` کپی کنید:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SnappDoctorDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Kavenegar": {
    "ApiKey": "YOUR_KAVENEGAR_API_KEY_HERE"
  }
}
```

### 3. نصب Packages
```bash
dotnet restore
```

### 4. ایجاد دیتابیس
```bash
cd src/SnappDoctor.Web
dotnet ef database update
```

### 5. اجرای پروژه
```bash
dotnet run
```

پروژه روی `https://localhost:5001` در دسترس خواهد بود.

## تنظیمات کاوه‌نگار

برای فعال‌سازی سرویس SMS:

1. در [پنل کاوه‌نگار](https://panel.kavenegar.com) ثبت‌نام کنید
2. API Key خود را دریافت کنید
3. در فایل `appsettings.Development.json` قرار دهید
4. یک الگوی پیامک با نام `verify` ایجاد کنید

## ساختار دیتابیس

### جداول اصلی:
- **AspNetUsers**: اطلاعات کاربران
- **Doctors**: اطلاعات پزشکان
- **Consultations**: مشاوره‌ها
- **OtpCodes**: کدهای تایید

### داده‌های نمونه:
پروژه شامل داده‌های نمونه برای پزشکان است که به طور خودکار در دیتابیس ایجاد می‌شود.

## API Endpoints

### Authentication
- `GET /Auth/Register` - صفحه ثبت‌نام
- `POST /Auth/Register` - ثبت‌نام کاربر
- `GET /Auth/Login` - صفحه ورود
- `POST /Auth/Login` - ورود کاربر
- `GET /Auth/VerifyOtp` - صفحه تایید کد
- `POST /Auth/VerifyOtp` - تایید کد OTP
- `POST /Auth/ResendOtp` - ارسال مجدد کد

### Home
- `GET /` - صفحه اصلی
- `GET /Home/About` - درباره ما

## ویژگی‌های امنیتی

- **Password Hashing**: رمزهای عبور با ASP.NET Identity hash می‌شوند
- **OTP Verification**: تایید شماره موبایل با کد یکبار مصرف
- **HTTPS**: اتصال امن
- **Input Validation**: اعتبارسنجی کامل ورودی‌ها
- **CSRF Protection**: محافظت در برابر حملات CSRF

## طراحی UI/UX

- **Responsive Design**: سازگار با تمام اندازه‌های صفحه
- **RTL Support**: پشتیبانی کامل از راست به چپ
- **Accessibility**: قابل دسترس برای همه کاربران
- **Modern UI**: طراحی مدرن با Tailwind CSS
- **Persian Typography**: فونت‌های بهینه فارسی

## مشارکت در پروژه

1. Fork کنید
2. شاخه جدید ایجاد کنید (`git checkout -b feature/amazing-feature`)
3. تغییرات را commit کنید (`git commit -m 'Add amazing feature'`)
4. Push کنید (`git push origin feature/amazing-feature`)
5. Pull Request ایجاد کنید

## لایسنس

این پروژه تحت لایسنس MIT منتشر شده است.

## تماس

- **ایمیل**: info@snappdoctor.ir
- **تلفن**: ۰۲۱-۱۲۳۴۵۶۷۸

---

**ساخته شده با ❤️ برای جامعه سلامت ایران** 