using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Infer.Models;
using Infer.Services;

namespace Infer.Controllers;

public class HomeController : Controller
{
    private readonly InferenceService _service;

    public HomeController(InferenceService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(string values)
    {
        var data = values.Split(',').Select(long.Parse).ToArray();
        if (data.Length != 64)
        {
            return View();
        }
        var value = _service.Infer(data);
        return View(value);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
