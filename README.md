# ManagementEvents

ManagementEvents is a full-stack event management system developed as a server-side web project.

The system allows users to view events, see event details and sessions, register for sessions, view their personal schedule, and see event statistics.  
The project also includes an admin area for creating, updating, and deleting events.

## Project Structure

The solution is divided into three projects:

text ManagementEvents ├── Client ├── Data └── ManagementEventsAPI 

### Client

The Client project contains the front-end files of the system.

It includes:

- HTML pages
- CSS styling
- JavaScript logic
- Fetch API calls to the Web API

Main pages:

- index.html - main dashboard
- EventDetails.html - event details and session registration
- Statistics.html - statistics dashboard
- MySchedule.html - user schedule page
- AdminLogin.html - admin login page
- Admin.html - admin management page

### Data

The Data project contains the data layer of the system.

It includes:

- Entity Framework Core DbContext
- Database models

Main folders:

text Data ├── Data │   └── EventSystemContext.cs └── Models     ├── Event.cs     ├── Session.cs     ├── SessionRegistration.cs     └── User.cs 

### ManagementEventsAPI

The ManagementEventsAPI project contains the Web API layer.

It includes:

- Controllers
- Services
- DTOs
- Program configuration
- Swagger support

Main folders:

text ManagementEventsAPI ├── Controllers ├── DTOs ├── Services └── Program.cs 

## Main Features

- View all events
- View event details
- View sessions for each event
- Register users to sessions
- Prevent duplicate registration
- Prevent registration to overlapping sessions
- View personal user schedule
- View event statistics
- Admin login
- Admin create event
- Admin update event
- Admin delete event
- Weather information for event location
- Swagger API documentation

## Technologies

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- HTML
- CSS
- JavaScript
- Fetch API
- Swagger
- Git and GitHub

## Database

The project uses a SQL Server database named:

text EventSystem 

The database script is included in the repository:

text ManagementSystem.sql 

The script creates the database, tables, relationships, and initial data.

Main tables:

- User
- Event
- Session
- SessionRegistration

## API Endpoints

### Event

text GET    /api/event/schedule GET    /api/event/{id} POST   /api/event PUT    /api/event/{id} DELETE /api/event/{id} GET    /api/event/{id}/weather POST   /api/event/{eventId}/session 

### Session

text POST /api/session/{sessionId}/register GET  /api/session/{sessionId}/user 

### User

text GET /api/user/{userId}/schedule 

## How to Run the Project

1. Open the solution file:

text ManagementEvents.sln 

2. Create the database by running:

text ManagementSystem.sql 

3. Run the Web API project:

text ManagementEventsAPI 

4. Open Swagger:

text https://localhost:5213/swagger 

or:

text http://localhost:5213/swagger 

5. Run the Client project and open the website.

## Admin Login

The system includes a simple admin login page for project demonstration.

text Username: admin Password: 1234 

## Notes

- The project is divided into Client, Data, and Web API layers.
- Entity Framework Core is used for database access.
- The Client communicates with the API using JavaScript Fetch API.
- Swagger is used for testing and documenting the API.
- The system includes validation for session registration, including duplicate and overlapping session checks.

## Author

Ofer Avioz  
Computer Engineering Student  
Ruppin Academic Center
