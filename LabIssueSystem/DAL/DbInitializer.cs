using Microsoft.EntityFrameworkCore;
using LabIssueSystem.Models;

namespace LabIssueSystem.DAL
{
    public class DbInitializer
    {
        public static void Initialize(LabIssueContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Users.Any())
            {
                return; // Data already seeded
            }

            // Create default users
            var users = new User[]
            {
                new User
                {
                    Username = "student1",
                    Password = "student123",
                    Role = "Student",
                    Email = "student1@lab.edu",
                    FullName = "John Student",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "network1",
                    Password = "network123",
                    Role = "NetworkTeam",
                    Email = "network1@lab.edu",
                    FullName = "Mike Network",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "faculty1",
                    Password = "faculty123",
                    Role = "Faculty",
                    Email = "faculty1@lab.edu",
                    FullName = "Dr. Faculty",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "student2",
                    Password = "student123",
                    Role = "Student",
                    Email = "student2@lab.edu",
                    FullName = "Jane Student",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "network2",
                    Password = "network123",
                    Role = "NetworkTeam",
                    Email = "network2@lab.edu",
                    FullName = "Sarah Network",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            };

            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();

            // Create sample tickets
            var tickets = new Ticket[]
            {
                new Ticket
                {
                    IPAddress = "192.168.1.101",
                    IssueTitle = "Cannot connect to internet",
                    IssueDescription = "The computer cannot access any websites. Network cable is connected but no internet access.",
                    Status = "Open",
                    Priority = "High",
                    ReportedDate = DateTime.Now.AddHours(-2),
                    ReportedBy = 1,
                    LabName = "Lab A",
                    ComputerName = "PC-101"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.102",
                    IssueTitle = "Printer not responding",
                    IssueDescription = "Unable to print documents. Printer shows offline status.",
                    Status = "InProgress",
                    Priority = "Medium",
                    ReportedDate = DateTime.Now.AddHours(-4),
                    ReportedBy = 4,
                    AssignedTo = 2,
                    LabName = "Lab A",
                    ComputerName = "PC-102"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.103",
                    IssueTitle = "Software installation needed",
                    IssueDescription = "Need Visual Studio 2022 installed for programming class.",
                    Status = "Resolved",
                    Priority = "Low",
                    ReportedDate = DateTime.Now.AddDays(-1),
                    ResolvedDate = DateTime.Now.AddHours(-2),
                    ReportedBy = 1,
                    AssignedTo = 2,
                    ResolutionNotes = "Visual Studio 2022 has been installed successfully.",
                    LabName = "Lab B",
                    ComputerName = "PC-103"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.104",
                    IssueTitle = "System running slow",
                    IssueDescription = "Computer is very slow and freezes frequently. Need virus scan and cleanup.",
                    Status = "Open",
                    Priority = "Medium",
                    ReportedDate = DateTime.Now.AddMinutes(-30),
                    ReportedBy = 4,
                    LabName = "Lab B",
                    ComputerName = "PC-104"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.105",
                    IssueTitle = "Monitor flickering",
                    IssueDescription = "The monitor flickers and goes black intermittently.",
                    Status = "InProgress",
                    Priority = "High",
                    ReportedDate = DateTime.Now.AddHours(-1),
                    ReportedBy = 1,
                    AssignedTo = 5,
                    LabName = "Lab C",
                    ComputerName = "PC-105"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.106",
                    IssueTitle = "Keyboard not working",
                    IssueDescription = "Some keys on the keyboard are not responding.",
                    Status = "Resolved",
                    Priority = "Medium",
                    ReportedDate = DateTime.Now.AddDays(-2),
                    ResolvedDate = DateTime.Now.AddDays(-1),
                    ReportedBy = 4,
                    AssignedTo = 2,
                    ResolutionNotes = "Keyboard has been replaced with a new one.",
                    LabName = "Lab C",
                    ComputerName = "PC-106"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.107",
                    IssueTitle = "USB ports not detecting devices",
                    IssueDescription = "USB ports are not detecting any devices plugged in.",
                    Status = "Open",
                    Priority = "Low",
                    ReportedDate = DateTime.Now.AddHours(-3),
                    ReportedBy = 1,
                    LabName = "Lab A",
                    ComputerName = "PC-107"
                },
                new Ticket
                {
                    IPAddress = "192.168.1.108",
                    IssueTitle = "Audio not working",
                    IssueDescription = "No sound output from speakers or headphones.",
                    Status = "Resolved",
                    Priority = "Medium",
                    ReportedDate = DateTime.Now.AddDays(-3),
                    ResolvedDate = DateTime.Now.AddDays(-2),
                    ReportedBy = 4,
                    AssignedTo = 5,
                    ResolutionNotes = "Audio drivers have been updated. Sound is now working.",
                    LabName = "Lab B",
                    ComputerName = "PC-108"
                }
            };

            foreach (var ticket in tickets)
            {
                context.Tickets.Add(ticket);
            }
            context.SaveChanges();

            // Create sample computers
            var computers = new Computer[]
            {
                new Computer { IPAddress = "192.168.1.101", ComputerName = "PC-101", LabName = "Lab A", Location = "Building 1, Room 101", IsActive = true },
                new Computer { IPAddress = "192.168.1.102", ComputerName = "PC-102", LabName = "Lab A", Location = "Building 1, Room 101", IsActive = true },
                new Computer { IPAddress = "192.168.1.103", ComputerName = "PC-103", LabName = "Lab B", Location = "Building 1, Room 102", IsActive = true },
                new Computer { IPAddress = "192.168.1.104", ComputerName = "PC-104", LabName = "Lab B", Location = "Building 1, Room 102", IsActive = true },
                new Computer { IPAddress = "192.168.1.105", ComputerName = "PC-105", LabName = "Lab C", Location = "Building 2, Room 201", IsActive = true },
                new Computer { IPAddress = "192.168.1.106", ComputerName = "PC-106", LabName = "Lab C", Location = "Building 2, Room 201", IsActive = true },
                new Computer { IPAddress = "192.168.1.107", ComputerName = "PC-107", LabName = "Lab A", Location = "Building 1, Room 101", IsActive = true },
                new Computer { IPAddress = "192.168.1.108", ComputerName = "PC-108", LabName = "Lab B", Location = "Building 1, Room 102", IsActive = true }
            };

            foreach (var computer in computers)
            {
                context.Computers.Add(computer);
            }
            context.SaveChanges();
        }
    }
}
