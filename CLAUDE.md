# Calendar Management API

## Project Overview

A web application and API for managing calendar events. Users can define three types of calendar items through a web interface and query events for specific dates via API.

## Implementation Status: Complete

All core features have been implemented and tested.

## Tech Stack

- .NET 10 Web API with Blazor Server
- Entity Framework Core with PostgreSQL
- Serilog for logging (daily rolling log files in `Logs/` directory)
- NUnit 4.x for testing with Moq and EF Core InMemory

## Solution Structure

```
CalendarManagementApi.sln
├── CalendarManagementApi/          # Main web application
│   ├── Components/                 # Blazor components
│   │   ├── App.razor               # Root component
│   │   ├── Routes.razor            # Router configuration
│   │   ├── _Imports.razor          # Global usings
│   │   ├── Layout/                 # Layout components
│   │   │   ├── MainLayout.razor    # Main layout with nav
│   │   │   └── NavMenu.razor       # Navigation component
│   │   └── Pages/                  # Page components
│   │       ├── Home.razor          # Dashboard with today's events
│   │       ├── DateEvents/         # DateEvent CRUD pages
│   │       ├── WaitingEvents/      # WaitingEvent CRUD pages
│   │       └── RepeatingEvents/    # RepeatingEvent CRUD pages
│   ├── Controllers/                # API endpoints
│   ├── Data/                       # EF Core DbContext
│   ├── DTOs/                       # Data transfer objects
│   ├── Models/                     # Entity models
│   ├── Services/                   # Business logic
│   │   ├── CalendarService.cs      # Calendar event retrieval
│   │   ├── IDateEventService.cs    # DateEvent interface
│   │   ├── DateEventService.cs     # DateEvent implementation
│   │   ├── IWaitingEventService.cs # WaitingEvent interface
│   │   ├── WaitingEventService.cs  # WaitingEvent implementation
│   │   ├── IRepeatingEventService.cs # RepeatingEvent interface
│   │   └── RepeatingEventService.cs  # RepeatingEvent implementation
│   ├── Migrations/                 # EF Core migrations
│   └── wwwroot/                    # Static files
│       └── css/app.css             # Application styles
└── CalendarManagementApi.Tests/    # NUnit test project
    ├── Controllers/                # Controller tests
    ├── Services/                   # Service tests
    └── Helpers/                    # Test utilities
```

## Event Types

### Date Events
Annual events on a specific month/day (e.g., birthdays, holidays).
- **Model:** `DateEvent` - Id, Name, Month (1-12), Day (1-31)
- **API:** `api/dateevents` - Full CRUD
- **UI:** `/dateevents` - List, Create, Edit, Delete pages

### Waiting Events
Events that occur once, then wait for user action to reschedule.
- **Model:** `WaitingEvent` - Id, Name, OccurrenceDate
- **API:** `api/waitingevents` - Full CRUD + postpone endpoints
  - `POST api/waitingevents/{id}/postpone-week` - Reschedule 7 days from today
  - `POST api/waitingevents/{id}/postpone-month` - Reschedule 1 month from today
- **UI:** `/waitingevents` - List, Create, Edit, Delete pages with postpone buttons

### Repeating Events
Events with configurable repeat patterns.
- **Model:** `RepeatingEvent` with `RepeatType` enum:
  - `DayOfWeek` - Every specified weekday (0=Sunday to 6=Saturday)
  - `DayOfWeekOfMonth` - Specified weekday on specific weeks (e.g., "1,3" for 1st and 3rd week)
  - `Interval` - Every X days from a start date
  - `Date` - Annually on specific month/day (similar to DateEvent but as repeating)
- **API:** `api/repeatingevents` - Full CRUD
- **UI:** `/repeatingevents` - List, Create, Edit, Delete pages with conditional form fields

## Calendar API

Query events for a specific date:
- `GET api/calendar/{date}` - Returns WaitingEvents (past due) and matching RepeatingEvents
- `GET api/calendar/dates/{date}` - Returns DateEvents matching the month/day

Response format:
```json
{
  "name": "Event Name",
  "eventType": "WaitingEvent|RepeatingEvent|DateEvent",
  "sourceId": 123
}
```

## Database

PostgreSQL with connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=calendardb;Username=...;Password=..."
  }
}
```

### Migrations
- `InitialCreate` - DateEvent, WaitingEvent, RepeatingEvent tables
- `AddDateRepeatType` - Added Month/Day fields to RepeatingEvent for Date repeat type

## Running the Application

```bash
# Build
dotnet build CalendarManagementApi.sln

# Run
dotnet run --project CalendarManagementApi

# Run tests (102 tests)
dotnet test CalendarManagementApi.sln
```

## Test Coverage

102 tests covering:
- **CalendarService** (46 tests): All repeat logic (CheckDayOfWeek, CheckDayOfWeekOfMonth, CheckInterval, CheckDate, GetWeekOfMonth)
- **Controllers** (56 tests): CRUD operations, 404 handling, IsPastDue calculation, postpone functionality
