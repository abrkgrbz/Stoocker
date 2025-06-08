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
├── Core/
│   └── Application/
│       └── Interfaces/
│           ├── IRepository<T>
│           ├── IReadRepository<T>
│           ├── IWriteRepository<T>
│           ├── IUnitOfWork
│           └── ISpecification<T>
└── Infrastructure/
└── Persistence/
└── Repositories/
├── ReadRepository<T>
├── WriteRepository<T>
├── SpecificationRepository<T>
└── UnitOfWork
