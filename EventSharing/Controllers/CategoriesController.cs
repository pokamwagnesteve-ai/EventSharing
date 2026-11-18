using EventSharing.Data;
using EventSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventSharing.Controllers;

/// <summary>Gère les catégories d'événements.</summary>
[Authorize]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Categories.AsNoTracking().OrderBy(category => category.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(item => item.Events)
            .FirstOrDefaultAsync(item => item.Id == id);

        return category is null ? NotFound() : View(category);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _context.Update(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(item => item.Events)
            .FirstOrDefaultAsync(item => item.Id == id);

        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.Include(item => item.Events).FirstOrDefaultAsync(item => item.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        if (category.Events.Count > 0)
        {
            TempData["Error"] = "Une catégorie utilisée par un événement ne peut pas être supprimée.";
            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
