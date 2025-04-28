using Microsoft.AspNetCore.Mvc;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(UsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = await _usuarioService.ObtenerUsuariosAsync();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return View("Error"); 
            }
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del usuario con ID {Id}.", id);
                return View("Error");
            }

        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usuarioService.CrearUsuarioAsync(usuario);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear un nuevo usuario.");
                    ModelState.AddModelError("", "No se pudo crear el usuario. Inténtalo de nuevo.");
                    return View(usuario);
                }
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario a editar con ID {Id}.", id);
                return View("Error");
            }
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _usuarioService.ActualizarUsuarioAsync(id, usuario);
                    if (!result)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar el usuario con ID {Id}.", id);
                    ModelState.AddModelError("", "No se pudo guardar los cambios. Inténtalo de nuevo.");
                    return View(usuario);
                }

            }
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario a eliminar con ID {Id}.", id);
                return View("Error");
            }

        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _usuarioService.EliminarUsuarioAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {Id}.", id);
                return View("Error");
            }

        }
    }
}

