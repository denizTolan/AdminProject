using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using AdminProject.Models;
using AdminProject.Models.Layout;
using Microsoft.AspNetCore.Authorization;

namespace AdminProject.Controllers;

[Authorize(Roles = "Admin,Default_Pages")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}