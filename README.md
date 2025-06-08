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
tech-stack:
  architecture: "Clean Architecture"
  patterns: 
    - "Repository Pattern"
    - "Unit of Work"
    - "Specification Pattern"
  
  structure:
    core:
      - type: "interfaces"
        location: "/Core/Application/Interfaces"
        components: ["IRepository", "IReadRepository", "IWriteRepository", "IUnitOfWork", "ISpecification"]
    
    infrastructure:
      - type: "implementations"
        location: "/Infrastructure/Persistence"
        components: ["ReadRepository", "WriteRepository", "SpecRepository", "UnitOfWork"]
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
