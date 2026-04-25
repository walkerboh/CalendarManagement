# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build (run from repo root, where the .sln lives)
dotnet build CalendarManagementApi.sln

# Run (from repo root)
dotnet run --project CalendarManagementApi/CalendarManagementApi

# All tests
dotnet test CalendarManagementApi.sln

# Single test class
dotnet test CalendarManagementApi.sln --filter "FullyQualifiedName~CalendarServiceTests"

# Add a migration (from CalendarManagementApi/ subdirectory)
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

The solution root is `E:\Development\calendar-management-api\`. The working directory for Claude Code is the `CalendarManagementApi/` subdirectory (the web project), so `.sln` commands need `../` or run from the parent.

## Tech Stack

- .NET 10 Blazor Server + Web API (same process)
- Entity Framework Core 10 with PostgreSQL (Npgsql)
- Serilog with daily rolling file sink (`Logs/` directory)
- NUnit 4 / Moq / EF Core InMemory for tests

## Architecture

### Dual surface: Blazor UI + REST API

Both surfaces live in one project. Blazor pages inject services directly (e.g., `@inject ICalendarService`). API controllers take the same services via constructor DI. There is no HTTP boundary between the Blazor UI and the database — Blazor pages call services directly, not the REST controllers.

### Service layer pattern

Every entity has an `IXxxService` interface and `XxxService` implementation registered as `Scoped`. Controllers and Blazor pages depend only on interfaces. `CalendarService` is the read-only query service used by the API and the Home dashboard.

### `IDateProvider` / `DateProvider`

All "what is today?" logic goes through `IDateProvider.Today` (returns `DateOnly`), registered as `Singleton`. Tests mock this to pin the current date. Never call `DateTime.Today` or `DateOnly.FromDateTime(DateTime.Today)` directly in services or controllers.

### Event types and data model

Four entity types, each with full CRUD:
- **MessageOfTheDay** — `(Month, Day)` unique index; displays a message once per year on that date
- **WaitingEvent** — one-shot event with `OccurrenceDate`; "past due" means `OccurrenceDate <= today`; postpone endpoints shift the date
- **RepeatingEvent** — polymorphic via `RepeatType` enum (`DayOfWeek`, `DayOfWeekOfMonth`, `Interval`, `Date`); only the fields relevant to the chosen type are populated
- **Birthday** — `(Month, Day)` with no uniqueness constraint; calendar query returns birthdays within a 14-day window

All four entities have an optional `Image` (string path/URL) and a `Layer` enum (`Black` / `Red`) used for display styling.

### Calendar query logic

`CalendarService` is the heart of the read path:
- `GetEventsForDate` returns due `WaitingEvent`s + `RepeatingEvent`s matching the date
- `GetMotdForDate` returns `MessageOfTheDay` rows matching `(month, day)`
- `GetBirthdaysForDate` returns birthdays within 14 days; EF can't translate tuple `Contains` to SQL so it fetches all rows and filters in memory

`DoesRepeatOnDate` dispatches to `CheckDayOfWeek`, `CheckDayOfWeekOfMonth`, `CheckInterval`, `CheckDate`. All these are `internal` to enable direct unit testing without the controller layer.

### Test infrastructure

- `TestDbContextFactory.Create()` spins up a unique EF InMemory database per test
- `ControllerTestBase<TController>` provides `Context`, `MockLogger`, and `MockDateProvider` with `SetUp`/`TearDown`; all controller test classes inherit from it
- Controller tests instantiate real services against the in-memory DB rather than mocking the service layer

## Database

Connection string in `appsettings.json` under `ConnectionStrings.DefaultConnection`. Migrations run automatically on startup via `dbContext.Database.Migrate()`.

Current migrations (in order): `InitialCreate` → `AddDateRepeatType` → `AddTextColorToDateEvent` → `AddImageToWaitingAndRepeatingEvents` → `RenameDateEventToMessageOfTheDay` → `AddLayerEnum` → `AddBirthdays`.

## Development Rules

- Always check unit tests and aim for full coverage
- Build and test after all code changes
