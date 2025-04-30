using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using VentasAPP.Models;
using System.Security.Claims;

namespace VentasAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       
        private readonly UsuarioService _usuarioService;
        // El controlador recibe  el logger y el servicio de usuarios
        public HomeController(ILogger<HomeController> logger , UsuarioService usuarioService)
        {
            _logger = logger;
            _usuarioService = usuarioService;
        }
        // Muestra la página de inicio (login)
        public IActionResult Index()
        {
            return View();
        }

        // Aqui se procesa el login del usuario
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // Valida credenciales de la API de usuarios
            var usuario = await _usuarioService.ValidarUsuarioAsync(loginViewModel.Correo, loginViewModel.Clave);
            if (usuario != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim("IDUsuario", usuario.IDUsuario.ToString())
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Producto");
                // Redirige a la lista de productos tras login exitoso
            }

            // Usuario no válido
            // agrega mensaje de error
            ModelState.AddModelError(string.Empty, "Correo o clave incorrectos.");
            return View(loginViewModel); 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Cierra la sesión del usuario
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
