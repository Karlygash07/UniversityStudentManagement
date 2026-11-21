using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityStudentManagement.Data;
using UniversityStudentManagement.ViewModels;

namespace UniversityStudentManagement.Controllers;

public sealed class HomeController(AppDbContext context, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var groupsWithStudents = await context.Groups.CountAsync(group => group.Students.Any(), cancellationToken);
            var model = new DashboardViewModel
            {
                TotalStudents = await context.Students.CountAsync(cancellationToken),
                TotalGroups = await context.Groups.CountAsync(cancellationToken),
                GroupsWithStudents = groupsWithStudents
            };
            return View(model);
        }
        catch (Exception exception) when (exception is Microsoft.Data.SqlClient.SqlException or InvalidOperationException)
        {
            logger.LogError(exception, "Could not load dashboard statistics.");
            TempData["Error"] = "The database is unavailable. Start SQL Server and apply the database migration, then refresh this page.";
            return View(new DashboardViewModel());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
