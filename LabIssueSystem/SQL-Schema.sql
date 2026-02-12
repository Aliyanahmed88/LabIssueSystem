-- ============================================
-- IP-Based Computer Lab Issue Reporting System
-- Database Schema SQL Script
-- ============================================

-- Create Database (if not exists)
-- IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'LabIssueDB')
-- BEGIN
--     CREATE DATABASE LabIssueDB
-- END
-- GO

-- Use LabIssueDB
-- USE LabIssueDB
-- GO

-- ============================================
-- Table: Users
-- ============================================
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Student', 'NetworkTeam', 'Faculty')),
    Email NVARCHAR(100) NOT NULL,
    FullName NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE(),
    IsActive BIT DEFAULT 1
);

-- ============================================
-- Table: Computers
-- ============================================
CREATE TABLE Computers (
    ComputerId INT IDENTITY(1,1) PRIMARY KEY,
    IPAddress NVARCHAR(15) NOT NULL UNIQUE,
    ComputerName NVARCHAR(50),
    LabName NVARCHAR(50),
    Location NVARCHAR(100),
    IsActive BIT DEFAULT 1
);

-- ============================================
-- Table: Tickets
-- ============================================
CREATE TABLE Tickets (
    TicketId INT IDENTITY(1,1) PRIMARY KEY,
    IPAddress NVARCHAR(15) NOT NULL,
    IssueTitle NVARCHAR(200) NOT NULL,
    IssueDescription NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Open' CHECK (Status IN ('Open', 'InProgress', 'Resolved')),
    Priority NVARCHAR(20) DEFAULT 'Medium' CHECK (Priority IN ('Low', 'Medium', 'High')),
    ReportedDate DATETIME DEFAULT GETDATE(),
    ResolvedDate DATETIME,
    ReportedBy INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    AssignedTo INT FOREIGN KEY REFERENCES Users(UserId),
    ResolutionNotes NVARCHAR(MAX),
    LabName NVARCHAR(50),
    ComputerName NVARCHAR(50)
);

-- ============================================
-- Sample Seed Data
-- ============================================

-- Insert Demo Users (Password stored as plain text)
INSERT INTO Users (Username, Password, Role, Email, FullName)
VALUES 
    ('student1', 'student123', 'Student', 'student1@lab.edu', 'John Student'),
    ('network1', 'network123', 'NetworkTeam', 'network1@lab.edu', 'Mike Network'),
    ('faculty1', 'faculty123', 'Faculty', 'faculty1@lab.edu', 'Dr. Faculty');

-- Insert Sample Computers
INSERT INTO Computers (IPAddress, ComputerName, LabName, Location)
VALUES 
    ('192.168.1.101', 'PC-101', 'Lab A', 'Building A - Room 101'),
    ('192.168.1.102', 'PC-102', 'Lab A', 'Building A - Room 101'),
    ('192.168.1.103', 'PC-103', 'Lab A', 'Building A - Room 101'),
    ('192.168.2.101', 'PC-201', 'Lab B', 'Building A - Room 201'),
    ('192.168.2.102', 'PC-202', 'Lab B', 'Building A - Room 201');

-- Insert Sample Tickets
INSERT INTO Tickets (IPAddress, IssueTitle, IssueDescription, Status, Priority, ReportedBy, LabName, ComputerName)
VALUES 
    ('192.168.1.101', 'No Internet Connection', 'Cannot access any websites or network resources', 'Open', 'High', 1, 'Lab A', 'PC-101'),
    ('192.168.1.102', 'Slow Performance', 'Computer is very slow and freezes frequently', 'InProgress', 'Medium', 1, 'Lab A', 'PC-102'),
    ('192.168.2.101', 'Software Error', 'Photoshop crashes when opening files', 'Resolved', 'Low', 1, 'Lab B', 'PC-201'),
    ('192.168.1.103', 'Printer Not Working', 'Cannot print documents to network printer', 'Open', 'High', 1, 'Lab A', 'PC-103');

-- Update resolved ticket with resolution notes
UPDATE Tickets 
SET ResolvedDate = GETDATE(), ResolutionNotes = 'Updated printer driver and configured network printer'
WHERE TicketId = 3;

-- ============================================
-- Useful Queries
-- ============================================

-- Get all tickets with user details
SELECT t.TicketId, t.IPAddress, t.IssueTitle, t.Status, t.Priority, 
       t.ReportedDate, t.ResolvedDate, u.Username AS ReportedBy,
       t.IssueDescription, t.ResolutionNotes
FROM Tickets t
INNER JOIN Users u ON t.ReportedBy = u.UserId
ORDER BY t.ReportedDate DESC;

-- Get tickets by status
SELECT * FROM Tickets WHERE Status = 'Open';
SELECT * FROM Tickets WHERE Status = 'InProgress';
SELECT * FROM Tickets WHERE Status = 'Resolved';

-- Get statistics
SELECT 
    COUNT(*) AS TotalTickets,
    SUM(CASE WHEN Status = 'Open' THEN 1 ELSE 0 END) AS OpenTickets,
    SUM(CASE WHEN Status = 'InProgress' THEN 1 ELSE 0 END) AS InProgressTickets,
    SUM(CASE WHEN Status = 'Resolved' THEN 1 ELSE 0 END) AS ResolvedTickets
FROM Tickets;

-- Get average resolution time (in hours)
SELECT 
    AVG(DATEDIFF(HOUR, ReportedDate, ResolvedDate)) AS AvgResolutionHours
FROM Tickets
WHERE Status = 'Resolved' AND ResolvedDate IS NOT NULL;

-- ============================================
-- Indexes for Performance
-- ============================================
CREATE INDEX IX_Tickets_Status ON Tickets(Status);
CREATE INDEX IX_Tickets_ReportedBy ON Tickets(ReportedBy);
CREATE INDEX IX_Tickets_IPAddress ON Tickets(IPAddress);
CREATE INDEX IX_Users_Role ON Users(Role);
