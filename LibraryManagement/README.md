# Library Management System

A simple console-based Library Management System built with .NET 8 using Clean Architecture principles.

The project demonstrates:

- Domain-Driven Design basics
- Layered architecture
- Repository pattern
- Dependency Injection
- JSON-based persistence
- Validation
- Logging
- Unit testing
- Error handling

This project was created as a backend-focused pet project for learning and demonstrating modern .NET development practices.

---

# Features

## Book Management

- Add books
- Update books
- Delete books (soft delete)
- Borrow books
- Return books
- Search books
- Show all books
- Show available books

## Validation

- Prevent duplicate book codes
- Validate required fields
- Prevent invalid operations
- Prevent borrowing deleted books

## Persistence

- JSON file storage
- Automatic file creation
- Automatic folder creation
- Soft delete support

## Logging

- Console logging
- Error logging
- Business operation logging

## Testing

- Domain tests
- Application tests
- Infrastructure tests

---

# Architecture

The solution follows a layered architecture inspired by Clean Architecture principles.

## Layers

### Domain

Contains:

- Entities
- Business rules
- Enums
- Domain exceptions

The Domain layer does not depend on any other project.

### Application

Contains:

- DTOs
- Services
- Validators
- Repository abstractions

Responsible for application use cases and orchestration.

### Infrastructure

Contains:

- JSON persistence
- Repository implementation
- File access logic
- Configuration

Responsible for technical implementation details.

### Console

Contains:

- Menus
- User input/output
- Dependency Injection setup

Acts as the Presentation layer.

---

# Project Structure

```text
LibraryManagement/
│
├── src/
│   ├── LibraryManagement.Console/
│   ├── LibraryManagement.Application/
│   ├── LibraryManagement.Domain/
│   └── LibraryManagement.Infrastructure/
│
├── tests/
│   └── LibraryManagement.Tests/
│
└── README.md
```

---

# Technologies

- .NET 8
- C#
- xUnit
- Moq
- FluentAssertions
- Microsoft Dependency Injection
- Microsoft Logging
- System.Text.Json

---

# How to Run

## Clone repository

```bash
git clone <repository-url>
```

## Navigate to project

```bash
cd LibraryManagement
```

## Build solution

```bash
dotnet build
```

## Run application

```bash
dotnet run --project src/LibraryManagement.Console/LibraryManagement.Console.csproj
```

---

# How to Test

Run all tests:

```bash
dotnet test
```

---

# Example Workflow

## Add book

1. Select menu option:

```text
1. Add book
```

2. Enter:

```text
Title
Author
Year
Code
```

3. Book will be saved to JSON storage.

---

# Storage

Books are stored in:

```text
data/books.json
```

The file is automatically created if it does not exist.

---

# Error Handling

The application handles:

- Validation errors
- Business rule violations
- JSON corruption
- File access errors
- Unexpected exceptions

User-friendly messages are displayed in the console.

---

# Logging

The application logs:

- Book creation
- Book updates
- Book deletion
- Borrow/return operations
- Storage errors
- Unexpected exceptions

Current logging target:

- Console

---

# Roadmap

## Planned Improvements

- SQL Server support
- Entity Framework Core
- ASP.NET Core Web API
- Swagger/OpenAPI
- Authentication & Authorization
- Docker support
- Pagination
- Advanced search
- Book reservations
- User management
- Borrowing history
- File-based logging
- Serilog integration

---

# Possible Future Improvements

## Architecture

- CQRS
- MediatR
- Result pattern
- Custom validation exceptions
- Separate persistence models
- Generic repository pattern

## Infrastructure

- SQLite support
- Redis caching
- Azure deployment
- Background jobs

## Testing

- Integration tests
- TestContainers
- Performance testing

---

# Learning Goals

This project was designed to practice:

- Clean code
- SOLID principles
- Layered architecture
- Unit testing
- Repository pattern
- Dependency Injection
- Modern .NET backend development

---

# Author

Oleksandr Ivchenko
Senior .NET Developer