# Generic Repository Pattern with Unit of Work

A clean architecture implementation of Repository Pattern with Unit of Work and Specification Pattern support.

## Features

- **Generic Repository Pattern**: Type-safe data access layer
- **Unit of Work**: Transaction management and repository coordination
- **Specification Pattern**: Encapsulate complex queries
- **Read/Write Separation**: Following CQRS principles
- **EF Core Integration**: Optimized for Entity Framework Core
- **Async/Await Support**: Full async operations with CancellationToken
- **Fluent API**: Chain tracking configuration

## Architecture
🏗️ Repository Pattern Implementation
│
├── 📦 Core Layer
│   └── 🔷 Application
│       └── 📁 Interfaces
│           ├── 🔹 IRepository<T>
│           ├── 🔹 IReadRepository<T>  
│           ├── 🔹 IWriteRepository<T>
│           ├── 🔹 IUnitOfWork
│           └── 🔹 ISpecification<T>
│
└── 📦 Infrastructure Layer
    └── 🔶 Persistence
        ├── 📁 Repositories
        │   ├── 📄 ReadRepository<T>
        │   ├── 📄 WriteRepository<T>
        │   ├── 📄 SpecificationRepository<T>
        │   └── 📄 UnitOfWork
        └── 📁 Specifications
            └── 📄 SpecificationEvaluator<T>
## Usage Example

```csharp
// Simple query
var product = await _unitOfWork
    .GetReadRepository<Product>()
    .AsNoTracking()
    .GetByIdAsync(productId);

// Complex query with specifications
var spec = new ProductWithCategorySpecification(minPrice: 100);
var products = await _unitOfWork
    .GetSpecificationRepository<Product>()
    .FindAsync(spec);

// Write operation
var writeRepo = _unitOfWork.GetWriteRepository<Product>();
await writeRepo.AddAsync(newProduct);
await _unitOfWork.SaveChangesAsync();
