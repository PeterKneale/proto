using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Infer.Models;
using Infer.Services;

namespace Infer.Controllers;

public class HomeController : Controller
{
    private readonly InferenceService _service;
    private readonly ILogger<HomeController> _logger;

    public HomeController(InferenceService service, ILogger<HomeController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public IActionResult Index()
    {
        long[] data = { 0 };
        var value = _service.Infer(data);
        return View(value);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
