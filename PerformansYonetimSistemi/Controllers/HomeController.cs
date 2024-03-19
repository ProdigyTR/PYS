using Microsoft.AspNetCore.Mvc;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models;
using PerformansYonetimSistemi.ViewModels;
using System.Diagnostics;

namespace PerformansYonetimSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            ViewBag.CurrentPage = "/Home/Index";
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
    }
}