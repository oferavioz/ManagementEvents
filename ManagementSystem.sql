CREATE DATABASE EventSystem;
GO
USE EventSystem;
GO

-- 1. User Table
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL
);

-- 2. Main Event Table (Conferences, Festivals, etc.)
CREATE TABLE [Event] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    EventType NVARCHAR(50) NOT NULL -- Conference, Festival, Party, etc.
);

-- 3. Sessions Table (Sub-Event, Lectures, Performances)
CREATE TABLE [Session] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES [Event](Id) ON DELETE CASCADE,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    SpeakerName NVARCHAR(100) NULL, -- Speaker, Lecturer or Artist
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    RoomName NVARCHAR(50) NULL      -- Specific hall, room, or stage
);

-- 4. Junction Table for [Session] Registrations (Many-to-Many Relationship)
-- The composite primary key strictly prEvent duplicate registrations for the same [Session].
CREATE TABLE SessionRegistration (
    SessionId INT NOT NULL FOREIGN KEY REFERENCES [Session](Id) ON DELETE CASCADE,
    UserId INT NOT NULL FOREIGN KEY REFERENCES [User](Id) ON DELETE CASCADE,
    RegistrationDate DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (SessionId, UserId)
);

-- 5. Seed Data (All Content in English)
INSERT INTO [User] (FullName, Email) VALUES 
('John Doe', 'john.doe@example.com'),
('Jane Smith', 'jane.smith@example.com'),
('Bob Johnson', 'bob.j@example.com');
('Ofer Avioz', 'ofer.avioz@example.com'),
('Daniel Levi', 'daniel.levi@example.com'),
('Noa Cohen', 'noa.cohen@example.com');

INSERT INTO [Event] (Title, Description, StartDate, EndDate, Location, EventType) VALUES 
('Modern Tech Conference 2026', 'A comprehensive conference covering software development and artificial intelligence.', '2026-06-15 09:00:00', '2026-06-15 17:00:00', 'Expo Tel Aviv', 'Conference'),
('Summer Music Festival 2026', 'An outdoor multi-stage music festival featuring international electronic and rock artists.', '2026-07-20 16:00:00', '2026-07-22 23:00:00', 'Yarkon Park Tel Aviv', 'Festival');

INSERT INTO [Session] (EventId, Title, Description, SpeakerName, StartTime, EndTime, RoomName) VALUES 
(1, 'Introduction to AI in Cloud', 'Keynote session exploring modern cloud-based artificial intelligence tools and LLMs.', 'Dr. John Doe', '2026-06-15 09:30:00', '2026-06-15 10:30:00', 'Hall A'),
(1, 'Microservices Architecture', 'A deep dive into server-side microservices design patterns and API gateways.', 'Eng. Ruth Levi', '2026-06-15 10:40:00', '2026-06-15 11:40:00', 'Hall B'),
(2, 'Opening Rock Live Act', 'An energetic live performance to kick off the summer festival lineup.', 'The Code Rockers', '2026-07-20 17:00:00', '2026-07-20 18:30:00', 'Main Stage');