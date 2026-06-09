# ManagementEvents

ManagementEvents is a server-side web application for managing events, sessions, user registrations, and personal schedules.

The system includes a Web API, a database layer, and a client-side interface.

## Project Structure

The solution is divided into three main projects:

| Project | Description |
|---|---|
| Client | Contains the client-side files: HTML, CSS, JavaScript, and Fetch API calls. |
| Data | Contains the Entity Framework Core database context and model classes. |
| ManagementEventsAPI | Contains the Web API controllers, services, DTOs, and application configuration. |

## Main Features

- View all events
- View event details and sessions
- Register users to sessions
- Prevent duplicate registrations
- Prevent registration to overlapping sessions
- View a user's personal schedule
- View event statistics
- Admin login
- Create, update, and delete events
- Display weather information by event location
- API testing and documentation using Swagger

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- HTML
- CSS
- JavaScript
- Fetch API
- Swagger
- GitHub

## Database

The project uses a SQL Server database named EventSystem.

The database creation and initialization script is included in the repository:

ManagementSystem.sql

Main database tables:

- User
- Event
- Session
- SessionRegistration

## How to Run the Project

1. Open the solution file:

   ManagementEvents.sln

2. Run the SQL script:

   ManagementSystem.sql

   This creates the database, tables, relationships, and initial data.

3. Run the Web API project:

   ManagementEventsAPI

4. Open Swagger in the browser:

   http://localhost:5213/swagger

5. Run the client project and open the website.

## Admin Login

For demonstration purposes, the system includes a simple admin login.

| Username | Password |
|---|---|
| admin | 1234 |

## Author

Ofer Avioz  
Computer Engineering Student  
Ruppin Academic Center
