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
    [Authorize(Roles = "NetworkTeam")]
    public class NetworkTeamController : Controller
    {
        private readonly LabIssueContext _context;

        public NetworkTeamController(LabIssueContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var tickets = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .ToListAsync();

            var resolvedTickets = tickets.Where(t => t.Status == "Resolved" && t.ResolvedDate.HasValue).ToList();
            double avgResolutionTime = 0;
            if (resolvedTickets.Any())
            {
                avgResolutionTime = resolvedTickets.Average(t => (t.ResolvedDate!.Value - t.ReportedDate).TotalHours);
            }

            var model = new DashboardViewModel
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == "Open"),
                InProgressTickets = tickets.Count(t => t.Status == "InProgress"),
                ResolvedTickets = tickets.Count(t => t.Status == "Resolved"),
                AverageResolutionTime = Math.Round(avgResolutionTime, 2),
                HighPriorityTickets = tickets.Count(t => t.Priority == "High" && t.Status != "Resolved"),
                MediumPriorityTickets = tickets.Count(t => t.Priority == "Medium" && t.Status != "Resolved"),
                LowPriorityTickets = tickets.Count(t => t.Priority == "Low" && t.Status != "Resolved"),
                RecentTickets = tickets.OrderByDescending(t => t.ReportedDate)
                    .Take(5)
                    .Select(t => new TicketViewModel
                    {
                        TicketId = t.TicketId,
                        IPAddress = t.IPAddress,
                        IssueTitle = t.IssueTitle,
                        Status = t.Status,
                        ReportedDate = t.ReportedDate,
                        Priority = t.Priority,
                        ReportedByName = t.Reporter?.FullName ?? t.Reporter?.Username
                    }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> ManageIssues(string? searchString, string? statusFilter, string? priorityFilter)
        {
            var tickets = _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t =>
                    t.TicketId.ToString().Contains(searchString) ||
                    t.IPAddress.Contains(searchString) ||
                    t.IssueTitle.Contains(searchString) ||
                    (t.Reporter != null && (t.Reporter.FullName != null && t.Reporter.FullName.Contains(searchString))) ||
                    (t.Reporter != null && t.Reporter.Username.Contains(searchString)));
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                tickets = tickets.Where(t => t.Status == statusFilter);
            }

            // Apply priority filter
            if (!string.IsNullOrEmpty(priorityFilter) && priorityFilter != "All")
            {
                tickets = tickets.Where(t => t.Priority == priorityFilter);
            }

            var ticketList = await tickets.OrderByDescending(t => t.ReportedDate).ToListAsync();

            ViewBag.StatusFilter = statusFilter;
            ViewBag.PriorityFilter = priorityFilter;
            ViewBag.SearchString = searchString;

            return View(ticketList);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Reporter)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new UpdateStatusViewModel
            {
                TicketId = ticket.TicketId,
                IPAddress = ticket.IPAddress,
                IssueTitle = ticket.IssueTitle,
                IssueDescription = ticket.IssueDescription,
                CurrentStatus = ticket.Status,
                NewStatus = ticket.Status,
                ResolutionNotes = ticket.ResolutionNotes
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticket = await _context.Tickets.FindAsync(model.TicketId);
            if (ticket == null)
            {
                return NotFound();
            }

            // Don't allow changing from Resolved back to Open/InProgress
            if (ticket.Status == "Resolved" && model.NewStatus != "Resolved")
            {
                ModelState.AddModelError("", "Cannot change status from Resolved to another status");
                return View(model);
            }

            // Require resolution notes when marking as Resolved
            if (model.NewStatus == "Resolved" && string.IsNullOrWhiteSpace(model.ResolutionNotes))
            {
                ModelState.AddModelError("ResolutionNotes", "Resolution notes are required when marking as Resolved");
                return View(model);
            }

            ticket.Status = model.NewStatus;
            ticket.ResolutionNotes = model.ResolutionNotes;

            if (model.NewStatus == "Resolved")
            {
                ticket.ResolvedDate = DateTime.Now;
            }

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ticket status updated successfully!";
            return RedirectToAction("ManageIssues");
        }

        public async Task<IActionResult> AssignToMe(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            ticket.AssignedTo = userId;
            ticket.Status = "InProgress";

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ticket assigned to you!";
            return RedirectToAction("ManageIssues");
        }

        public async Task<IActionResult> TicketDetails(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Reporter)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketId == id);

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
                ResolvedDate = ticket.ResolvedDate,
                ReportedByName = ticket.Reporter?.FullName ?? ticket.Reporter?.Username,
                AssignedToName = ticket.Assignee?.FullName ?? ticket.Assignee?.Username,
                ResolutionNotes = ticket.ResolutionNotes,
                LabName = ticket.LabName,
                ComputerName = ticket.ComputerName
            };

            return View(model);
        }
    }
}
