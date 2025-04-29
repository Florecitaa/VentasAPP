using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _usuarioService;
        public HomeController(ILogger<HomeController> logger , HomeService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError($"Error de validación: {error.ErrorMessage}");
                }
                return View(loginViewModel); 
            }
            bool esValido = await _usuarioService.ValidarUsuarioAsync(loginViewModel);
            if (esValido)
            {
                return RedirectToAction("Index", "Producto");
            }
            ModelState.AddModelError(string.Empty, "Correo o clave incorrectos.");
            return View(loginViewModel); 
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
