# Timesheet Management System

A full-stack application demonstrating enterprise-level architecture patterns, built with **.NET 8 Web API** (backend) and **Angular 19** (frontend).

> 📚 **Learning Goal**: This project is designed to help developers understand Clean Architecture, Design Patterns, Entity Framework Code-First, and State Management with NgRx and Angular Signals.

---

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Tech Stack](#tech-stack)
- [Architecture Overview](#architecture-overview)
- [Backend Architecture (Clean Architecture)](#backend-architecture-clean-architecture)
- [Design Patterns Explained](#design-patterns-explained)
- [Entity Framework Code-First Approach](#entity-framework-code-first-approach)
- [Frontend Architecture (Angular 19)](#frontend-architecture-angular-19)
- [NgRx State Management](#ngrx-state-management)
- [Angular Signals](#angular-signals)
- [Trade-offs & Design Decisions](#trade-offs--design-decisions)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)

---

## Project Overview

This Timesheet Management System allows:
- **Employees** to create and submit timesheets with time entries
- **Managers** to approve/reject timesheets, manage projects, and assign employees
- **Reports** for tracking hours by employee, project, and billable status

---

## Tech Stack

### Backend
- .NET 8 Web API
- Entity Framework Core 8.0 (Code-First)
- SQL Server LocalDB
- AutoMapper

### Frontend
- Angular 19 (Standalone Components)
- NgRx (Store, Effects, Entity)
- Angular Signals
- SCSS

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            FRONTEND (Angular 19)                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │ Components  │──│  Services   │──│  NgRx Store │──│   Signals   │    │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘    │
└────────────────────────────────┬────────────────────────────────────────┘
                                 │ HTTP (REST API)
┌────────────────────────────────┴────────────────────────────────────────┐
│                          BACKEND (.NET 8 API)                            │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                    API Layer (Controllers)                       │    │
│  └────────────────────────────┬────────────────────────────────────┘    │
│  ┌────────────────────────────┴────────────────────────────────────┐    │
│  │              Application Layer (Services, DTOs)                  │    │
│  └────────────────────────────┬────────────────────────────────────┘    │
│  ┌────────────────────────────┴────────────────────────────────────┐    │
│  │               Domain Layer (Entities, Enums)                     │    │
│  └────────────────────────────┬────────────────────────────────────┘    │
│  ┌────────────────────────────┴────────────────────────────────────┐    │
│  │         Infrastructure Layer (EF Core, Repositories)            │    │
│  └─────────────────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────────────┘
```

---

## Backend Architecture (Clean Architecture)

Clean Architecture separates code into layers with clear dependencies. **Inner layers know nothing about outer layers.**

### 📁 Project Structure

```
api/src/
├── Timesheet.Domain/           # Inner-most layer (no dependencies)
│   ├── Entities/               # Business entities
│   ├── Enums/                  # Business enumerations
│   └── Common/                 # Base classes
│
├── Timesheet.Application/      # Business logic layer
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Interfaces/             # Contracts (abstractions)
│   ├── Mappings/               # AutoMapper profiles
│   └── Services/               # Business services
│
├── Timesheet.Infrastructure/   # External concerns
│   ├── Data/                   # DbContext, configurations
│   └── Repositories/           # Data access implementations
│
└── Timesheet.Api/              # Entry point
    ├── Controllers/            # HTTP endpoints
    └── Program.cs              # Application startup
```

### Layer Responsibilities

#### 1. Domain Layer (`Timesheet.Domain`)
**Purpose**: Contains business entities and rules. Has **zero dependencies** on other projects.

```csharp
// Example: Entity with business rules
public class Timesheet : BaseEntity
{
    public int UserId { get; set; }
    public DateTime SubmissionDate { get; set; }
    public TimesheetStatus Status { get; set; }  // Draft, Submitted, Approved, Rejected
    public string? RejectionComments { get; set; }
    
    // Navigation properties (relationships)
    public virtual User User { get; set; } = null!;
    public virtual ICollection<TimesheetEntry> Entries { get; set; } = new List<TimesheetEntry>();
}
```

**Why?** Business logic should be independent of databases, UI, or frameworks.

#### 2. Application Layer (`Timesheet.Application`)
**Purpose**: Orchestrates business operations, defines interfaces, and maps data.

```csharp
// Interface (contract) - defined here
public interface ITimesheetService
{
    Task<TimesheetDto?> CreateAsync(int userId, CreateTimesheetDto dto);
    Task<bool> SubmitAsync(int timesheetId);
    Task<bool> ApproveAsync(int timesheetId);
}

// Service (implementation) - also here
public class TimesheetService : ITimesheetService
{
    private readonly ITimesheetRepository _repository;
    
    public async Task<bool> ApproveAsync(int timesheetId)
    {
        var timesheet = await _repository.GetByIdAsync(timesheetId);
        if (timesheet?.Status != TimesheetStatus.Submitted)
            return false;
        
        timesheet.Status = TimesheetStatus.Approved;
        await _repository.UpdateAsync(timesheet);
        return true;
    }
}
```

**DTOs (Data Transfer Objects)**: Objects that carry data between layers.

```csharp
// What API receives from client
public class CreateTimesheetDto
{
    public DateTime SubmissionDate { get; set; }
    public List<CreateTimesheetEntryDto> Entries { get; set; }
}

// What API sends to client
public class TimesheetDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public DateTime SubmissionDate { get; set; }
    public decimal TotalHours { get; set; }  // Calculated field
}
```

**Why DTOs?**
- Don't expose entity structure to API consumers
- Can shape data differently than database structure
- Add calculated fields without changing entities

#### 3. Infrastructure Layer (`Timesheet.Infrastructure`)
**Purpose**: Implements interfaces defined in Application layer. Handles database operations.

```csharp
// Repository implementation
public class TimesheetRepository : ITimesheetRepository
{
    private readonly AppDbContext _context;
    
    public async Task<Timesheet?> GetByIdAsync(int id)
    {
        return await _context.Timesheets
            .Include(t => t.User)
            .Include(t => t.Entries)
                .ThenInclude(e => e.Project)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
```

#### 4. API Layer (`Timesheet.Api`)
**Purpose**: HTTP endpoints, handles requests/responses, dependency injection setup.

```csharp
[ApiController]
[Route("api/[controller]")]
public class TimesheetsController : ControllerBase
{
    private readonly ITimesheetService _service;
    
    [HttpPost("user/{userId}")]
    public async Task<ActionResult<TimesheetDto>> Create(int userId, CreateTimesheetDto dto)
    {
        var result = await _service.CreateAsync(userId, dto);
        return result != null ? Ok(result) : BadRequest();
    }
}
```

---

## Design Patterns Explained

### 1. Repository Pattern

**What it is**: A layer between business logic and data access. Provides a collection-like interface for accessing domain objects.

**Why use it?**
- Hides database complexity from business logic
- Makes code testable (can mock repositories)
- Centralizes data access logic

```csharp
// Interface (in Application layer)
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

// Implementation (in Infrastructure layer)
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
```

**Real-world analogy**: Think of a repository like a librarian. You ask the librarian for a book, and they know where to find it. You don't need to know how the library is organized.

### 2. Unit of Work Pattern

**What it is**: Maintains a list of objects affected by a business transaction and coordinates writing out changes.

**Why use it?**
- Groups multiple operations into a single transaction
- Ensures data consistency
- Allows rolling back all changes if something fails

```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    ITimesheetRepository Timesheets { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

// Usage example
public async Task TransferEmployee(int userId, int fromProject, int toProject)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        await _unitOfWork.ProjectAssignments.RemoveFromProject(userId, fromProject);
        await _unitOfWork.ProjectAssignments.AddToProject(userId, toProject);
        await _unitOfWork.CommitAsync();  // Both succeed or both fail
    }
    catch
    {
        await _unitOfWork.RollbackAsync();  // Undo everything
        throw;
    }
}
```

**Real-world analogy**: Like a shopping cart. You add multiple items, and when you checkout, either the entire order processes or nothing does.

### 3. Factory Pattern

**What it is**: Creates objects without exposing creation logic to the client.

**Why use it?**
- Centralizes object creation
- Can create different types based on conditions
- Easy to add new types without changing existing code

```csharp
// Factory for creating services based on user role
public interface IServiceFactory
{
    ITimesheetService CreateTimesheetService(UserRole role);
}

public class ServiceFactory : IServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public ITimesheetService CreateTimesheetService(UserRole role)
    {
        // Return different service implementations based on role
        return role switch
        {
            UserRole.Manager => _serviceProvider.GetRequiredService<ManagerTimesheetService>(),
            UserRole.Employee => _serviceProvider.GetRequiredService<EmployeeTimesheetService>(),
            _ => throw new ArgumentException("Unknown role")
        };
    }
}
```

**Real-world analogy**: Like ordering food at a restaurant. You tell the waiter what you want, and the kitchen (factory) creates it. You don't know or care how it's made.

### 4. Strategy Pattern

**What it is**: Defines a family of algorithms, encapsulates each one, and makes them interchangeable.

**Why use it?**
- Avoids large if-else/switch statements
- Easy to add new behaviors
- Runtime algorithm selection

```csharp
// Interface defining the strategy contract
public interface IHoursCalculationStrategy
{
    decimal CalculateTotalHours(IEnumerable<TimesheetEntry> entries);
}

// Different strategies for different calculation needs
public class StandardHoursStrategy : IHoursCalculationStrategy
{
    public decimal CalculateTotalHours(IEnumerable<TimesheetEntry> entries)
    {
        return entries.Sum(e => (decimal)e.Hours);
    }
}

public class OvertimeHoursStrategy : IHoursCalculationStrategy
{
    public decimal CalculateTotalHours(IEnumerable<TimesheetEntry> entries)
    {
        decimal total = 0;
        decimal dailyHours = 0;
        
        foreach (var entry in entries.OrderBy(e => e.Date))
        {
            dailyHours += (decimal)entry.Hours;
            if (dailyHours > 8)
            {
                total += (decimal)entry.Hours * 1.5m;  // Overtime pay
            }
            else
            {
                total += (decimal)entry.Hours;
            }
        }
        return total;
    }
}

// Context that uses the strategy
public class TimesheetCalculator
{
    private IHoursCalculationStrategy _strategy;
    
    public void SetStrategy(IHoursCalculationStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public decimal Calculate(IEnumerable<TimesheetEntry> entries)
    {
        return _strategy.CalculateTotalHours(entries);
    }
}
```

**Real-world analogy**: Like choosing a route on Google Maps. You pick "fastest", "shortest", or "avoid tolls" - each is a different strategy to reach your destination.

### 5. Decorator Pattern

**What it is**: Adds behavior to objects dynamically without affecting other objects.

**Why use it?**
- Add features without modifying existing code
- Combine behaviors flexibly
- Follow Open/Closed Principle (open for extension, closed for modification)

```csharp
// Base service interface
public interface ITimesheetService
{
    Task<TimesheetDto?> CreateAsync(int userId, CreateTimesheetDto dto);
}

// Core implementation
public class TimesheetService : ITimesheetService
{
    public async Task<TimesheetDto?> CreateAsync(int userId, CreateTimesheetDto dto)
    {
        // Core creation logic
        return await CreateTimesheetInDatabase(userId, dto);
    }
}

// Decorator that adds logging
public class LoggingTimesheetService : ITimesheetService
{
    private readonly ITimesheetService _inner;
    private readonly ILogger<LoggingTimesheetService> _logger;
    
    public LoggingTimesheetService(ITimesheetService inner, ILogger<LoggingTimesheetService> logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public async Task<TimesheetDto?> CreateAsync(int userId, CreateTimesheetDto dto)
    {
        _logger.LogInformation("Creating timesheet for user {UserId}", userId);
        var result = await _inner.CreateAsync(userId, dto);
        _logger.LogInformation("Created timesheet {TimesheetId}", result?.Id);
        return result;
    }
}

// Decorator that adds validation
public class ValidatingTimesheetService : ITimesheetService
{
    private readonly ITimesheetService _inner;
    
    public ValidatingTimesheetService(ITimesheetService inner)
    {
        _inner = inner;
    }
    
    public async Task<TimesheetDto?> CreateAsync(int userId, CreateTimesheetDto dto)
    {
        // Validation before calling inner service
        if (dto.Entries.Any(e => e.Hours > 24))
            throw new ArgumentException("Cannot log more than 24 hours per entry");
            
        if (dto.Entries.Any(e => e.Date > DateTime.Today))
            throw new ArgumentException("Cannot log hours for future dates");
            
        return await _inner.CreateAsync(userId, dto);
    }
}

// Registration in Program.cs - decorators wrap each other
services.AddScoped<TimesheetService>();  // Core
services.AddScoped<ITimesheetService>(sp =>
{
    var core = sp.GetRequiredService<TimesheetService>();
    var validating = new ValidatingTimesheetService(core);
    var logging = new LoggingTimesheetService(validating, sp.GetRequiredService<ILogger<LoggingTimesheetService>>());
    return logging;  // Logging → Validation → Core
});
```

**Real-world analogy**: Like adding toppings to coffee. Start with espresso (base), add milk (decorator 1), add whipped cream (decorator 2). Each topping adds something without changing the coffee itself.

---

## Entity Framework Code-First Approach

### What is Code-First?

Instead of designing database tables first, you write C# classes and EF creates the database structure from them.

### Step 1: Define Entities

```csharp
// Entity = Database Table
public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties define relationships
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
}

public class Project : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public bool IsBillable { get; set; }
    public ProjectStatus Status { get; set; }
    
    public virtual ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
}

// Junction table for many-to-many relationship
public class ProjectAssignment : BaseEntity
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Foreign key navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}
```

### Step 2: Configure with Fluent API

```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }
    public DbSet<TimesheetEntry> TimesheetEntries { get; set; }
    public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);  // Primary key
            
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.HasIndex(e => e.Email)
                .IsUnique();  // Email must be unique
        });
        
        // Relationship: User has many Timesheets
        modelBuilder.Entity<Timesheet>(entity =>
        {
            entity.HasOne(t => t.User)
                .WithMany(u => u.Timesheets)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete
        });
        
        // Many-to-Many: Users <-> Projects (through ProjectAssignment)
        modelBuilder.Entity<ProjectAssignment>(entity =>
        {
            entity.HasOne(pa => pa.User)
                .WithMany(u => u.ProjectAssignments)
                .HasForeignKey(pa => pa.UserId);
                
            entity.HasOne(pa => pa.Project)
                .WithMany(p => p.Assignments)
                .HasForeignKey(pa => pa.ProjectId);
        });
    }
}
```

### Step 3: Migrations

```bash
# Create migration (generates SQL from your C# changes)
dotnet ef migrations add InitialCreate

# Apply to database
dotnet ef database update
```

### Include() for Loading Related Data

```csharp
// Without Include - only loads Timesheet data
var timesheet = await _context.Timesheets
    .FirstOrDefaultAsync(t => t.Id == id);
// timesheet.User is NULL!
// timesheet.Entries is EMPTY!

// With Include - loads related data (Eager Loading)
var timesheet = await _context.Timesheets
    .Include(t => t.User)                    // Load User
    .Include(t => t.Entries)                 // Load Entries collection
        .ThenInclude(e => e.Project)         // For each Entry, load its Project
    .FirstOrDefaultAsync(t => t.Id == id);
// Now timesheet.User has data
// timesheet.Entries has all entries with their Projects
```

---

## Frontend Architecture (Angular 19)

### 📁 Project Structure

```
web/src/app/
├── core/                       # Singleton services, models, guards
│   ├── models/                 # TypeScript interfaces
│   ├── services/               # HTTP services
│   └── guards/                 # Route guards
│
├── features/                   # Feature modules (lazy-loaded)
│   ├── auth/                   # Login feature
│   ├── manager/                # Manager dashboard
│   └── timesheet/              # Employee timesheet
│
├── shared/                     # Reusable components
│   └── components/
│
├── store/                      # NgRx state management
│   ├── project/
│   └── timesheet/
│
└── app.config.ts               # Application configuration
```

### Standalone Components (Angular 19)

Angular 19 uses standalone components by default - no NgModules needed!

```typescript
// Traditional Angular (with modules)
@NgModule({
    declarations: [LoginComponent],
    imports: [CommonModule, ReactiveFormsModule],
})
export class AuthModule {}

// Angular 19 Standalone (no module needed!)
@Component({
    selector: 'app-login',
    standalone: true,  // Self-contained component
    imports: [CommonModule, ReactiveFormsModule],  // Imports at component level
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss',
})
export class LoginComponent {
    // Component logic
}
```

**Benefits of Standalone Components:**
- Simpler mental model (no modules to manage)
- Better tree-shaking (smaller bundles)
- Each component declares its own dependencies

---

## NgRx State Management

### What is NgRx?

NgRx is Redux for Angular - a pattern for managing application state in a predictable way.

### Core Concepts

```
┌──────────────────────────────────────────────────────────────┐
│                        COMPONENT                              │
│   dispatch(action) ──────────────────────► select(state)     │
└─────────┬────────────────────────────────────────▲───────────┘
          │                                        │
          ▼                                        │
┌─────────────────┐                     ┌─────────────────────┐
│     ACTIONS     │                     │      SELECTORS      │
│  (What happened)│                     │  (Query the state)  │
└────────┬────────┘                     └──────────▲──────────┘
         │                                         │
         ▼                                         │
┌─────────────────┐      ┌───────────────────────────────────┐
│    REDUCERS     │─────►│              STORE                 │
│ (How state      │      │  (Single source of truth)         │
│  changes)       │      └───────────────────────────────────┘
└─────────────────┘                    │
                                       │
┌──────────────────────────────────────▼───────────────────────┐
│                         EFFECTS                               │
│  (Side effects: API calls, localStorage, etc.)               │
└──────────────────────────────────────────────────────────────┘
```

### 1. Actions - What Happened

```typescript
// Actions describe events that happen in your app
import { createAction, props } from '@ngrx/store';
import { Timesheet } from '../../core/models';

// Action with no payload
export const loadTimesheets = createAction(
    '[Timesheet] Load Timesheets'
);

// Action with payload
export const loadTimesheetsSuccess = createAction(
    '[Timesheet] Load Timesheets Success',
    props<{ timesheets: Timesheet[] }>()
);

// Action for errors
export const loadTimesheetsFailure = createAction(
    '[Timesheet] Load Timesheets Failure',
    props<{ error: string }>()
);

// Action with parameters
export const submitTimesheet = createAction(
    '[Timesheet] Submit',
    props<{ timesheetId: number }>()
);
```

**Naming Convention**: `[Feature] Event Description`
- `[Timesheet] Load Timesheets` - Component requested data
- `[Timesheet API] Load Timesheets Success` - API returned successfully

### 2. Reducers - How State Changes

```typescript
// Reducers are pure functions that take current state + action → new state
import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';

// Define state shape
export interface TimesheetState extends EntityState<Timesheet> {
    loading: boolean;
    error: string | null;
}

// EntityAdapter provides helper functions for collections
export const adapter: EntityAdapter<Timesheet> = createEntityAdapter<Timesheet>();

// Initial state
export const initialState: TimesheetState = adapter.getInitialState({
    loading: false,
    error: null,
});

// Reducer function
export const timesheetReducer = createReducer(
    initialState,
    
    // When loading starts, set loading = true
    on(TimesheetActions.loadTimesheets, (state) => ({
        ...state,
        loading: true,
        error: null,
    })),
    
    // When data arrives, add to collection
    on(TimesheetActions.loadTimesheetsSuccess, (state, { timesheets }) =>
        adapter.setAll(timesheets, { ...state, loading: false })
    ),
    
    // When error occurs, store error message
    on(TimesheetActions.loadTimesheetsFailure, (state, { error }) => ({
        ...state,
        loading: false,
        error,
    })),
    
    // When timesheet is submitted, update its status
    on(TimesheetActions.submitTimesheetSuccess, (state, { timesheet }) =>
        adapter.updateOne(
            { id: timesheet.id, changes: timesheet },
            state
        )
    )
);
```

**Key Rule**: Reducers must be PURE functions
- No side effects (API calls, console.log, etc.)
- Same input always produces same output
- Never mutate state directly, always return new state

### 3. Effects - Side Effects (API Calls)

```typescript
// Effects handle async operations
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, mergeMap, of } from 'rxjs';

@Injectable()
export class TimesheetEffects {
    private actions$ = inject(Actions);
    private timesheetService = inject(TimesheetService);
    
    // Effect: When loadTimesheets action is dispatched, call API
    loadTimesheets$ = createEffect(() =>
        this.actions$.pipe(
            ofType(TimesheetActions.loadTimesheets),      // Listen for this action
            mergeMap(() =>
                this.timesheetService.getAll().pipe(       // Call API
                    map(timesheets => 
                        TimesheetActions.loadTimesheetsSuccess({ timesheets })  // Success action
                    ),
                    catchError(error =>
                        of(TimesheetActions.loadTimesheetsFailure({ error: error.message }))  // Error action
                    )
                )
            )
        )
    );
    
    // Effect: Submit timesheet
    submitTimesheet$ = createEffect(() =>
        this.actions$.pipe(
            ofType(TimesheetActions.submitTimesheet),
            mergeMap(({ timesheetId }) =>
                this.timesheetService.submit(timesheetId).pipe(
                    map(timesheet => 
                        TimesheetActions.submitTimesheetSuccess({ timesheet })
                    ),
                    catchError(error =>
                        of(TimesheetActions.submitTimesheetFailure({ error: error.message }))
                    )
                )
            )
        )
    );
}
```

**Flow:**
1. Component dispatches `loadTimesheets` action
2. Effect catches it, calls API
3. API returns → Effect dispatches `loadTimesheetsSuccess`
4. Reducer updates state with new data
5. Component receives updated data through selector

### 4. Selectors - Query the State

```typescript
// Selectors are functions that extract data from state
import { createFeatureSelector, createSelector } from '@ngrx/store';

// Select the feature state
export const selectTimesheetState = createFeatureSelector<TimesheetState>('timesheets');

// Select all timesheets (using EntityAdapter)
export const { selectAll: selectAllTimesheets } = adapter.getSelectors(selectTimesheetState);

// Select loading state
export const selectTimesheetLoading = createSelector(
    selectTimesheetState,
    (state) => state.loading
);

// Select error state
export const selectTimesheetError = createSelector(
    selectTimesheetState,
    (state) => state.error
);

// Derived selector: Pending timesheets only
export const selectPendingTimesheets = createSelector(
    selectAllTimesheets,
    (timesheets) => timesheets.filter(t => t.status === TimesheetStatus.Submitted)
);

// Derived selector: Total hours
export const selectTotalHours = createSelector(
    selectAllTimesheets,
    (timesheets) => timesheets.reduce((sum, t) => sum + t.totalHours, 0)
);
```

**Benefits of Selectors:**
- Memoized (cached until input changes)
- Composable (build complex queries from simple ones)
- Reusable across components

### Using NgRx in Components

```typescript
@Component({
    selector: 'app-timesheet-list',
    standalone: true,
    imports: [CommonModule],
    template: `
        @if (loading()) {
            <div class="spinner">Loading...</div>
        } @else {
            @for (timesheet of timesheets(); track timesheet.id) {
                <div class="timesheet-card">
                    <h3>{{ timesheet.submissionDate | date }}</h3>
                    <p>{{ timesheet.totalHours }} hours</p>
                    <button (click)="submit(timesheet.id)">Submit</button>
                </div>
            }
        }
    `
})
export class TimesheetListComponent implements OnInit {
    private store = inject(Store);
    
    // Select data as signals (Angular 19)
    timesheets = this.store.selectSignal(selectAllTimesheets);
    loading = this.store.selectSignal(selectTimesheetLoading);
    
    ngOnInit() {
        // Dispatch action to load data
        this.store.dispatch(TimesheetActions.loadTimesheets());
    }
    
    submit(id: number) {
        this.store.dispatch(TimesheetActions.submitTimesheet({ timesheetId: id }));
    }
}
```

---

## Angular Signals

### What are Signals?

Signals are Angular's new reactive primitive for fine-grained reactivity. Think of them as "reactive variables."

### Signal vs Observable

```typescript
// OLD WAY: BehaviorSubject + async pipe
export class CounterComponent {
    count$ = new BehaviorSubject<number>(0);
    
    increment() {
        this.count$.next(this.count$.value + 1);
    }
}
// In template: {{ count$ | async }}

// NEW WAY: Signals
export class CounterComponent {
    count = signal(0);
    
    increment() {
        this.count.update(c => c + 1);
    }
}
// In template: {{ count() }}
```

### Signal API

```typescript
import { signal, computed, effect } from '@angular/core';

// Create a signal
const name = signal('John');

// Read value (call it like a function)
console.log(name());  // 'John'

// Update value
name.set('Jane');                    // Direct set
name.update(n => n.toUpperCase());   // Update based on current value

// Computed signals (derived values)
const greeting = computed(() => `Hello, ${name()}!`);
console.log(greeting());  // 'Hello, JANE!'

// Effects (side effects when signals change)
effect(() => {
    console.log(`Name changed to: ${name()}`);
    // Runs automatically when name() changes
});
```

### Signals with NgRx Store

```typescript
@Component({
    selector: 'app-dashboard',
    template: `
        <div class="stats">
            <div class="stat">
                <span class="value">{{ draftCount() }}</span>
                <span class="label">Draft</span>
            </div>
            <div class="stat">
                <span class="value">{{ submittedCount() }}</span>
                <span class="label">Submitted</span>
            </div>
        </div>
    `
})
export class DashboardComponent {
    private store = inject(Store);
    
    // Store data as signal
    timesheets = this.store.selectSignal(selectAllTimesheets);
    
    // Computed signals for derived data
    draftCount = computed(() => 
        this.timesheets().filter(t => t.status === TimesheetStatus.Draft).length
    );
    
    submittedCount = computed(() =>
        this.timesheets().filter(t => t.status === TimesheetStatus.Submitted).length
    );
}
```

### Local Component State with Signals

```typescript
@Component({
    selector: 'app-timesheet-form',
    template: `
        <form>
            @if (showForm()) {
                <div class="modal">
                    <!-- form fields -->
                    <button (click)="showForm.set(false)">Cancel</button>
                </div>
            }
            <button (click)="showForm.set(true)">Add Entry</button>
        </form>
    `
})
export class TimesheetFormComponent {
    // Local UI state
    showForm = signal(false);
    selectedProject = signal<Project | null>(null);
    entries = signal<TimesheetEntry[]>([]);
    
    addEntry(entry: TimesheetEntry) {
        this.entries.update(current => [...current, entry]);
    }
    
    removeEntry(index: number) {
        this.entries.update(current => current.filter((_, i) => i !== index));
    }
}
```

---

## Trade-offs & Design Decisions

### 1. Clean Architecture Complexity

**Trade-off**: More files and layers vs. maintainability

| Pros | Cons |
|------|------|
| Clear separation of concerns | More boilerplate code |
| Easy to test each layer | Steeper learning curve |
| Framework-independent business logic | Simple CRUD takes more code |
| Easy to swap implementations | Need to understand all layers |

**When to use**: Medium to large projects, teams with multiple developers.

**When to avoid**: Simple CRUD apps, prototypes, one-person projects.

### 2. Repository + Unit of Work

**Trade-off**: Abstraction vs. simplicity

| Pros | Cons |
|------|------|
| Hides EF Core from business logic | May duplicate EF Core features |
| Easy to mock for testing | Extra layer of indirection |
| Centralizes data access patterns | Repositories can become bloated |

**Alternative**: Use EF Core directly in services for simpler apps.

### 3. NgRx State Management

**Trade-off**: Predictability vs. boilerplate

| Pros | Cons |
|------|------|
| Predictable state changes | Lots of boilerplate code |
| Great debugging (Redux DevTools) | Learning curve |
| Clear data flow | Overkill for simple apps |
| State immutability | Async operations are complex |

**When to use**: Complex state, shared state between components, time-travel debugging needed.

**Alternative**: Angular Signals for simpler state, services with BehaviorSubject for medium complexity.

### 4. Decorator Pattern for Services

**Trade-off**: Flexibility vs. complexity

| Pros | Cons |
|------|------|
| Add behaviors without changing core code | Complex DI registration |
| Single Responsibility Principle | Harder to trace code flow |
| Easy to toggle features | Order of decorators matters |

**Example in this project**: Logging → Validation → Core Service

### 5. Angular Standalone Components

**Trade-off**: Simplicity vs. shared imports

| Pros | Cons |
|------|------|
| No NgModules needed | Must import CommonModule in every component |
| Clear dependencies per component | Can lead to repetitive imports |
| Better tree-shaking | Less familiar to Angular veterans |

### 6. EF Core Code-First

**Trade-off**: Developer productivity vs. DBA control

| Pros | Cons |
|------|------|
| Database from C# classes | Less control over schema |
| Migrations track changes | Complex migrations can be tricky |
| No SQL knowledge needed | Generated SQL may not be optimal |

**Alternative**: Database-First for existing databases or when DBAs own schema.

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- SQL Server LocalDB (comes with Visual Studio)

### Backend Setup

```bash
# Navigate to API project
cd api/src/Timesheet.Api

# Restore packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

API runs at: `http://localhost:5052`

### Frontend Setup

```bash
# Navigate to web project
cd web

# Install dependencies
npm install

# Start development server
ng serve
```

Frontend runs at: `http://localhost:4200`

### Test Users

| Email | Password | Role |
|-------|----------|------|
| admin@timesheet.com | Admin@123 | Manager |
| john@timesheet.com | User@123 | Employee |

---

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `GET /api/users/employees` - Get employees only

### Projects
- `GET /api/projects` - Get all projects
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}/toggle-status` - Activate/Deactivate

### Project Assignments
- `GET /api/projectassignments` - Get all assignments
- `POST /api/projectassignments` - Create assignment
- `GET /api/projectassignments/user/{userId}` - Get user's assignments

### Timesheets
- `GET /api/timesheets/user/{userId}` - Get user's timesheets
- `POST /api/timesheets/user/{userId}` - Create timesheet
- `PUT /api/timesheets/{id}/submit` - Submit for approval
- `PUT /api/timesheets/{id}/approve` - Approve (Manager only)
- `PUT /api/timesheets/{id}/reject` - Reject (Manager only)
- `GET /api/timesheets/pending` - Get pending timesheets

### Reports
- `GET /api/reports/employee-hours` - Hours by employee
- `GET /api/reports/project-hours` - Hours by project
- `GET /api/reports/billable` - Billable vs non-billable

---

## Further Learning

### Design Patterns
- [Refactoring Guru - Design Patterns](https://refactoring.guru/design-patterns)
- [Head First Design Patterns](https://www.oreilly.com/library/view/head-first-design/0596007124/)

### Clean Architecture
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Jason Taylor's Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)

### NgRx
- [NgRx Documentation](https://ngrx.io/)
- [NgRx Best Practices](https://ngrx.io/guide/store/configuration/runtime-checks)

### Angular Signals
- [Angular Signals Guide](https://angular.io/guide/signals)

---

## License

MIT License - Feel free to use this for learning!
