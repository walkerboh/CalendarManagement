# Calendar Management API

## Project Overview

This will be basic web app and API allowing users to define three different types of items to appear on a calendar. The user should be able to open a web page that will display the different kinds of items, and then within each type of item, they should be able to create new, edit existing, and delete items. In addition to the web app, there should be an API endpoint that allows external applications to request the events that should be displayed on a given day. The three types of items are as follows:

### Date Events

Date events fall on a specific date of the year. The only configuration for them should be the name of the event and the month and day the event occurs on.

### Waiting events

Waiting events are events that should occur on specific dates, but are then updated to occur again at a specifed later date only upon user action. When creating an event it requires a name and an occurence date. Once the occurence date is passed, it should be returned in the API until a user changes the next occurrence. When viewing such an event a viewer should be given buttons to set the next occurence to 7 days later or 1 month later.

### Repeating events

Repeating events are events that occur at a specified interval. There are multiple kinds of intervals:

#### Day of week

The specified event will occur on the weekday specified. So for example it could occur every Monday.

#### Day of week of month

The specifed event will occur on the weekday specified but only for certain weeks of the month. For example you could have an event that occurs on Saturday but only the first and third week of the month.

#### Interval

The specified event will occur on a starting date and then will repeat every X days.

## Tech Stack

- .NET 10 Web API
- Entity Framework Core with Postgresql
- Use controllers for the API endpoints
- Serilog for logging using a daily rolling log file