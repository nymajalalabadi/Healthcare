using Microsoft.AspNetCore.Mvc;
using SnappDoctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SnappDoctor.Web.Models;
using System.Diagnostics;

namespace SnappDoctor.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var doctors = await _context.Doctors
            .Where(d => d.IsActive && d.IsAvailable)
            .Take(6)
            .ToListAsync();

        ViewBag.Doctors = doctors;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Services()
    {
        return View();
    }

    public IActionResult Doctors()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
} 