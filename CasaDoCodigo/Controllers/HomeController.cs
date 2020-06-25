using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CasaDoCodigo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Página de descrição de sua aplicação.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Sua página de Contato.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
