using Microsoft.AspNetCore.Mvc;
using Projet.Models;

namespace Projet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Page d'accueil
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
