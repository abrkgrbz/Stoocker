# 🚀 Generic Repository Pattern with Unit of Work

[![.NET](https://img.shields.io/badge/.NET-9+-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9+-512BD4?style=flat-square&logo=microsoft)](https://docs.microsoft.com/en-us/ef/core/)
[![C#](https://img.shields.io/badge/C%23-10+-239120?style=flat-square&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

# Stoocker - Stok Yönetim Sistemi

Stoocker, .NET 9 tabanlı, Clean Architecture prensiplerine göre geliştirilmiş modern bir stok yönetim sistemidir. Multi-tenant mimariye sahip olan bu sistem, işletmelerin stok takibi, satış ve satın alma operasyonlarını verimli bir şekilde yönetmelerini sağlar.

## 🚀 Özellikler

- **Multi-Tenant Mimari**: Birden fazla müşterinin aynı uygulama üzerinde çalışabilmesi
- **Clean Architecture**: Katmanlı mimari ile sürdürülebilir ve test edilebilir kod yapısı
- **CQRS Pattern**: MediatR kullanarak komut ve sorgu işlemlerinin ayrılması
- **Identity Management**: ASP.NET Core Identity ile kullanıcı ve rol yönetimi
- **Background Jobs**: Hangfire ile arka plan işlemleri
- **Monitoring**: OpenTelemetry, Prometheus ve Health Checks ile sistem izleme
- **Logging**: Serilog ile yapılandırılabilir loglama (Console, File, Elasticsearch, Seq)
- **Validation**: FluentValidation ile veri doğrulama
- **Auto Mapping**: AutoMapper ile nesne dönüştürme
- **API Documentation**: Swagger ile API dokümantasyonu

## 🏗️ Proje Mimarisi

Proje Clean Architecture prensiplerine uygun olarak 5 ana katmandan oluşur:

```
Stoocker/
├── Stoocker.API/                          # Presentation Layer
├── Stoocker.Application/                  # Application Layer
├── Stoocker.Domain/                       # Domain Layer
├── Stoocker.Infrastructure/               # Infrastructure Layer
├── Stoocker.Infrastructure.BackgroundJobs/# Background Jobs
└── Stoocker.Persistence/                  # Data Access Layer
```

### Katman Detayları

#### 🎯 **Stoocker.API** (Presentation Layer)
- Web API endpoints
- Swagger konfigürasyonu
- Middleware konfigürasyonu
- Dependency injection container

#### 🔧 **Stoocker.Application** (Application Layer)
- CQRS pattern implementasyonu (MediatR)
- Business logic
- Validation (FluentValidation)
- Services interfaces ve implementasyonları
- AutoMapper profiles
- Pipeline behaviors (Validation, Logging)

#### 🏛️ **Stoocker.Domain** (Domain Layer)
- Entity modelleri
- Domain events
- Business rules
- Value objects

#### 🔌 **Stoocker.Infrastructure** (Infrastructure Layer)
- External service integrations
- Monitoring (OpenTelemetry, Prometheus)
- Health checks
- Logging configuration (Serilog)
- Background job configuration (Hangfire)

#### 💾 **Stoocker.Persistence** (Data Access Layer)
- Entity Framework Core configuration
- Repository pattern
- Unit of Work pattern
- Database migrations
- Tenant-aware repositories

## 🛠️ Teknoloji Stack

### Backend
- **.NET 9**: Framework
- **ASP.NET Core**: Web API
- **Entity Framework Core**: ORM
- **SQL Server**: Database
- **MediatR**: CQRS implementation
- **AutoMapper**: Object mapping
- **FluentValidation**: Validation
- **Hangfire**: Background jobs
- **Serilog**: Logging

### Authentication & Authorization
- **ASP.NET Core Identity**: User management
- **JWT Bearer**: Token authentication
- **BCrypt.Net**: Password hashing

### Monitoring & Observability
- **OpenTelemetry**: Distributed tracing
- **Prometheus**: Metrics collection
- **Health Checks**: Application health monitoring
- **Serilog Sinks**: Multiple logging destinations

### Documentation & Testing
- **Swagger/OpenAPI**: API documentation
- **Swashbuckle**: Swagger UI

## 📋 Gereksinimler

- .NET 9 SDK
- SQL Server (LocalDB veya tam sürüm)
- Visual Studio 2022 veya JetBrains Rider
- Git

## ⚡ Kurulum

### 1. Projeyi Klonlayın
```bash
git clone <repository-url>
cd Stoocker
```

### 2. Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 3. Veritabanı Konfigürasyonu
`appsettings.json` dosyasında connection string'i ayarlayın:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StoockerDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 4. Veritabanı Migration'larını Uygulayın
```bash
cd Stoocker.Persistence
dotnet ef database update
```

### 5. Uygulamayı Çalıştırın
```bash
cd Stoocker.API
dotnet run
```

Uygulama `https://localhost:7000` adresinde çalışacaktır.

## 📊 API Dokümantasyonu

Uygulama çalıştığında Swagger UI'ya şu adresten erişebilirsiniz:
- **Swagger UI**: `https://localhost:7000/swagger`

## 🏢 Domain Modelleri

Sistem aşağıdaki ana domain modellerini içerir:

- **Tenant**: Multi-tenant yapı için müşteri bilgileri
- **ApplicationUser**: Kullanıcı bilgileri
- **ApplicationRole**: Rol bilgileri
- **Brand**: Marka bilgileri
- **Category**: Kategori bilgileri
- **Product**: Ürün bilgileri
- **ProductStock**: Ürün stok bilgileri
- **Customer**: Müşteri bilgileri
- **Supplier**: Tedarikçi bilgileri
- **Warehouse**: Depo bilgileri
- **Unit**: Birim bilgileri
- **PurchaseInvoice**: Satın alma faturaları
- **SalesInvoice**: Satış faturaları
- **StockMovement**: Stok hareketleri

## 🔐 Authentication

Sistem JWT tabanlı authentication kullanır. API endpoint'lerine erişim için:

1. `/api/auth/login` endpoint'ine credentials gönderin
2. Dönen JWT token'ı `Authorization: Bearer <token>` header'ında kullanın

## 📈 Monitoring

### Health Checks
- Application health: `/health`
- Database health check
- Hangfire health check

### Metrics
- Prometheus metrics: `/metrics`
- OpenTelemetry tracing aktif

### Logging
Serilog ile çoklu sink desteği:
- Console logging
- File logging
- Elasticsearch
- Seq

## 🔄 Background Jobs

Hangfire ile arka plan işlemleri:
- Dashboard: `/hangfire`
- Otomatik job scheduling
- Recurring jobs

## 🏗️ Geliştirme

### Yeni Feature Ekleme

1. **Domain**: Yeni entity'leri `Stoocker.Domain` projesine ekleyin
2. **Application**: Business logic'i `Stoocker.Application` projesine ekleyin
3. **Persistence**: Repository ve configuration'ları ekleyin
4. **API**: Controller ve endpoint'leri ekleyin

### CQRS Pattern Kullanımı

```csharp
// Command örneği
public class CreateBrandCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}

// Query örneği
public class GetBrandByIdQuery : IRequest<Result<BrandDto>>
{
    public int Id { get; set; }
}
```

## 🧪 Testing

Proje test edilebilir mimari için tasarlanmıştır:
- Repository pattern ile testable data access
- Dependency injection ile loosely coupled design
- MediatR handlers için unit testing

## 📝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'i push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 🤝 Destek

Sorularınız için:
- Issue açın
- Dokümantasyonu kontrol edin
- Community'ye katılın

---

**Stoocker** - Modern stok yönetimi için geliştirilmiş, ölçeklenebilir ve sürdürülebilir bir çözüm.
---

<p align="center">
Made with ❤️ by BerkGrbzSoftEng
</p>
