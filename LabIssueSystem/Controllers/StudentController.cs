using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LabIssueSystem.DAL;
using LabIssueSystem.Models;
using LabIssueSystem.Models.ViewModels;

namespace LabIssueSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly LabIssueContext _context;

        public StudentController(LabIssueContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var tickets = await _context.Tickets
                .Where(t => t.ReportedBy == userId)
                .ToListAsync();


            var model = new DashboardViewModel
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == "Open"),
                InProgressTickets = tickets.Count(t => t.Status == "InProgress"),
                ResolvedTickets = tickets.Count(t => t.Status == "Resolved"),
                RecentTickets = tickets.OrderByDescending(t => t.ReportedDate)
                    .Take(5)
                    .Select(t => new TicketViewModel
                    {
                        TicketId = t.TicketId,
                        IPAddress = t.IPAddress,
                        IssueTitle = t.IssueTitle,
                        Status = t.Status,
                        ReportedDate = t.ReportedDate,
                        Priority = t.Priority
                    }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ReportIssue()
        {
            var model = new ReportIssueViewModel();
            model.IPAddress = GetClientIPAddress();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportIssue(ReportIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var ticket = new Ticket
            {
                IPAddress = model.IPAddress,
                IssueTitle = model.IssueTitle,
                IssueDescription = model.IssueDescription,
                LabName = model.LabName,
                ComputerName = model.ComputerName,
                Priority = model.Priority,
                Status = "Open",
                ReportedDate = DateTime.Now,
                ReportedBy = userId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Issue reported successfully! Your ticket ID is " + ticket.TicketId;
            return RedirectToAction("ViewStatus");
        }

        public async Task<IActionResult> ViewStatus(string? searchString, string? statusFilter)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // First get all tickets for this user
            var allTickets = await _context.Tickets
                .Where(t => t.ReportedBy == userId)
                .Include(t => t.Reporter)
                .ToListAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                allTickets = allTickets.Where(t => 
                    t.TicketId.ToString().Contains(searchString) ||
                    (t.IPAddress != null && t.IPAddress.Contains(searchString)) ||
                    (t.IssueTitle != null && t.IssueTitle.Contains(searchString))).ToList();
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                allTickets = allTickets.Where(t => t.Status == statusFilter).ToList();
            }

            var ticketList = allTickets.OrderByDescending(t => t.ReportedDate).ToList();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.SearchString = searchString;

            return View(ticketList);
        }

        public async Task<IActionResult> TicketDetails(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var ticket = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketId == id && t.ReportedBy == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new TicketViewModel
            {
                TicketId = ticket.TicketId,
                IPAddress = ticket.IPAddress,
                IssueTitle = ticket.IssueTitle,
                IssueDescription = ticket.IssueDescription,
                Status = ticket.Status,
                Priority = ticket.Priority,
                ReportedDate = ticket.ReportedDate,
                ReportedByName = ticket.Reporter?.FullName ?? ticket.Reporter?.Username,
                AssignedToName = ticket.Assignee?.FullName ?? ticket.Assignee?.Username,
                ResolutionNotes = ticket.ResolutionNotes,
                LabName = ticket.LabName,
                ComputerName = ticket.ComputerName
            };

            return View(model);
        }

        private string GetClientIPAddress()
        {
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            return remoteIp ?? "127.0.0.1";
        }
    }
}
