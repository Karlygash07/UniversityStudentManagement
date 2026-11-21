using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniversityStudentManagement.Data;
using UniversityStudentManagement.Models;
using UniversityStudentManagement.ViewModels;

namespace UniversityStudentManagement.Controllers;

public sealed class StudentsController(AppDbContext context, ILogger<StudentsController> logger) : Controller
{
    public async Task<IActionResult> Index(string? searchTerm, CancellationToken cancellationToken)
    {
        var query = context.Students.Include(student => student.Group).AsNoTracking();
        var normalizedSearch = searchTerm?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
            query = query.Where(student => student.Name.Contains(normalizedSearch));

        try
        {
            var model = new StudentIndexViewModel
            {
                Students = await query.OrderBy(student => student.Name).ToListAsync(cancellationToken),
                SearchTerm = normalizedSearch,
                TotalStudents = await context.Students.CountAsync(cancellationToken),
                TotalGroups = await context.Groups.CountAsync(cancellationToken)
            };
            return View(model);
        }
        catch (Exception exception) when (exception is Microsoft.Data.SqlClient.SqlException or InvalidOperationException)
        {
            logger.LogError(exception, "Could not load student records.");
            TempData["Error"] = "Student records could not be loaded. Check the database connection and migration.";
            return View(new StudentIndexViewModel { SearchTerm = normalizedSearch });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        try { return View(await BuildFormAsync(new StudentFormViewModel(), cancellationToken)); }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not load student groups for registration.");
            TempData["Error"] = "The database is unavailable. Start SQL Server and apply the migration before registering students.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentFormViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            if (!await context.Groups.AnyAsync(group => group.Id == model.GroupId, cancellationToken))
                ModelState.AddModelError(nameof(model.GroupId), "Choose a valid group.");
        }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not validate the selected student group.");
            TempData["Error"] = "The database is unavailable. Your student details were not saved.";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            var student = new Student
            {
                Name = model.Name.Trim(),
                Email = model.Email.Trim(),
                GroupId = model.GroupId
            };
            context.Students.Add(student);
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                TempData["Success"] = $"{student.Name} was added to the student directory.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException exception)
            {
                logger.LogWarning(exception, "Student could not be created.");
                ModelState.AddModelError(nameof(model.Email), "This email may already be registered. Check the address and try again.");
            }
        }

        return View(await BuildFormAsync(model, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        Student? student;
        try { student = await context.Students.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken); }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not load student {StudentId}.", id);
            TempData["Error"] = "The database is unavailable. Student details could not be loaded.";
            return RedirectToAction(nameof(Index));
        }
        if (student is null) return NotFound();

        var model = new StudentFormViewModel { Id = student.Id, Name = student.Name, Email = student.Email, GroupId = student.GroupId };
        return View(await BuildFormAsync(model, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        Student? student;
        try
        {
            if (!await context.Groups.AnyAsync(group => group.Id == model.GroupId, cancellationToken))
                ModelState.AddModelError(nameof(model.GroupId), "Choose a valid group.");
            student = await context.Students.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not load student {StudentId} for update.", id);
            TempData["Error"] = "The database is unavailable. Student details were not updated.";
            return RedirectToAction(nameof(Index));
        }
        if (student is null) return NotFound();

        if (ModelState.IsValid)
        {
            student.Name = model.Name.Trim();
            student.Email = model.Email.Trim();
            student.GroupId = model.GroupId;
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                TempData["Success"] = $"{student.Name}'s details were updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException exception)
            {
                logger.LogWarning(exception, "Student {StudentId} changed during edit.", id);
                ModelState.AddModelError(string.Empty, "This record changed while you were editing it. Refresh and try again.");
            }
            catch (DbUpdateException exception)
            {
                logger.LogWarning(exception, "Student {StudentId} could not be updated.", id);
                ModelState.AddModelError(nameof(model.Email), "This email may already belong to another student.");
            }
        }

        return View(await BuildFormAsync(model, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        Student? student;
        try
        {
            student = await context.Students.Include(item => item.Group).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not load student {StudentId} for confirmation.", id);
            TempData["Error"] = "The database is unavailable. Student details could not be loaded.";
            return RedirectToAction(nameof(Index));
        }
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        Student? student;
        try { student = await context.Students.FirstOrDefaultAsync(item => item.Id == id, cancellationToken); }
        catch (Exception exception) when (IsDatabaseUnavailable(exception))
        {
            logger.LogError(exception, "Could not load student {StudentId} for removal.", id);
            TempData["Error"] = "The database is unavailable. The student was not removed.";
            return RedirectToAction(nameof(Index));
        }
        if (student is null) return NotFound();

        context.Students.Remove(student);
        await context.SaveChangesAsync(cancellationToken);
        TempData["Success"] = "Student was removed from the directory.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<StudentFormViewModel> BuildFormAsync(StudentFormViewModel model, CancellationToken cancellationToken)
    {
        model.Groups = await context.Groups.AsNoTracking().OrderBy(group => group.Name)
            .Select(group => new SelectListItem(group.Name, group.Id.ToString()))
            .ToListAsync(cancellationToken);
        if (!model.Groups.Any())
            ModelState.AddModelError(nameof(model.GroupId), "No groups are available. Apply the database migration to add the starter groups.");
        return model;
    }

    private static bool IsDatabaseUnavailable(Exception exception) =>
        exception is Microsoft.Data.SqlClient.SqlException or InvalidOperationException;
}
