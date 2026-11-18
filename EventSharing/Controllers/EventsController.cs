using EventSharing.Data;
using EventSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventSharing.Controllers;

/// <summary>Gère la consultation et la gestion des événements.</summary>
public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public EventsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>Affiche les événements à venir.</summary>
    public async Task<IActionResult> Index(int? categoryId)
    {
        IQueryable<Event> query = _context.Events
            .AsNoTracking()
            .Include(eventItem => eventItem.Category)
            .Include(eventItem => eventItem.Creator)
            .Include(eventItem => eventItem.Participants)
            .OrderBy(eventItem => eventItem.StartDate);

        if (categoryId.HasValue)
        {
            query = query.Where(eventItem => eventItem.CategoryId == categoryId.Value);
        }

        ViewBag.Categories = await _context.Categories.AsNoTracking().OrderBy(category => category.Name).ToListAsync();
        ViewBag.SelectedCategoryId = categoryId;
        return View(await query.ToListAsync());
    }

    /// <summary>Affiche le détail d'un événement.</summary>
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.Creator)
            .Include(item => item.Participants)
            .FirstOrDefaultAsync(item => item.Id == id);

        return eventItem is null ? NotFound() : View(eventItem);
    }

    [Authorize]
    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,CategoryId")] Event eventItem)
    {
        RemoveServerManagedValidation();
        ValidateDates(eventItem);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(eventItem.CategoryId);
            return View(eventItem);
        }

        eventItem.CreatorId = _userManager.GetUserId(User)!;
        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = eventItem.Id });
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!IsOwner(eventItem))
        {
            return Forbid();
        }

        await PopulateCategoriesAsync(eventItem.CategoryId);
        return View(eventItem);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,CategoryId")] Event eventItem)
    {
        if (id != eventItem.Id)
        {
            return NotFound();
        }

        var existingEvent = await _context.Events.FindAsync(id);
        if (existingEvent is null)
        {
            return NotFound();
        }

        if (!IsOwner(existingEvent))
        {
            return Forbid();
        }

        RemoveServerManagedValidation();
        ValidateDates(eventItem);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(eventItem.CategoryId);
            return View(eventItem);
        }

        existingEvent.Name = eventItem.Name;
        existingEvent.Description = eventItem.Description;
        existingEvent.StartDate = eventItem.StartDate;
        existingEvent.EndDate = eventItem.EndDate;
        existingEvent.CategoryId = eventItem.CategoryId;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var eventItem = await _context.Events
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (eventItem is null)
        {
            return NotFound();
        }

        return IsOwner(eventItem) ? View(eventItem) : Forbid();
    }

    [HttpPost, ActionName("Delete")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem is null)
        {
            return NotFound();
        }

        if (!IsOwner(eventItem))
        {
            return Forbid();
        }

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Join(int id)
    {
        var eventItem = await _context.Events.Include(item => item.Participants).FirstOrDefaultAsync(item => item.Id == id);
        var user = await _userManager.GetUserAsync(User);
        if (eventItem is null || user is null)
        {
            return NotFound();
        }

        if (!eventItem.Participants.Any(participant => participant.Id == user.Id))
        {
            eventItem.Participants.Add(user);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Leave(int id)
    {
        var eventItem = await _context.Events.Include(item => item.Participants).FirstOrDefaultAsync(item => item.Id == id);
        var user = await _userManager.GetUserAsync(User);
        if (eventItem is null || user is null)
        {
            return NotFound();
        }

        var participant = eventItem.Participants.FirstOrDefault(item => item.Id == user.Id);
        if (participant is not null)
        {
            eventItem.Participants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    private bool IsOwner(Event eventItem) => eventItem.CreatorId == _userManager.GetUserId(User);

    private async Task PopulateCategoriesAsync(int? selectedCategoryId = null)
    {
        ViewBag.CategoryId = new SelectList(
            await _context.Categories.AsNoTracking().OrderBy(category => category.Name).ToListAsync(),
            nameof(Category.Id),
            nameof(Category.Name),
            selectedCategoryId);
    }

    private void ValidateDates(Event eventItem)
    {
        if (eventItem.EndDate <= eventItem.StartDate)
        {
            ModelState.AddModelError(nameof(Event.EndDate), "La date de fin doit être postérieure à la date de début.");
        }
    }

    private void RemoveServerManagedValidation()
    {
        ModelState.Remove(nameof(Event.Category));
        ModelState.Remove(nameof(Event.Creator));
        ModelState.Remove(nameof(Event.CreatorId));
        ModelState.Remove(nameof(Event.Participants));
    }
}
