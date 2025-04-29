using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class VentaController : Controller
    {
        private readonly VentaService _ventaService;
        private readonly ILogger<VentaController> _logger;
        public VentaController(VentaService ventaService, ILogger<VentaController> logger)
        {
            _ventaService = ventaService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var ventas = await _ventaService.ObtenerTodasLasVentasAsync();
                return View(ventas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de ventas.");
                return View("Error"); 
            }
        }

        // GET: Venta/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
                if (venta == null)
                {
                    return NotFound();
                }
                return View(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles de la venta con ID {Id}.", id);
                return View("Error");
            }
        }

        // GET: Venta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venta venta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var idVenta = await _ventaService.InsertarVentaAsync(venta);
                    // Manejar el idVenta como sea necesario, por ejemplo, mostrar un mensaje de éxito
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear una nueva venta.");
                    ModelState.AddModelError("", "No se pudo crear la venta. Inténtalo de nuevo.");
                    return View(venta);
                }
            }
            return View(venta);
        }

        // GET: Venta/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
                if (venta == null)
                {
                    return NotFound();
                }
                return View(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la venta a editar con ID {Id}.", id);
                return View("Error");
            }
        }

        // POST: Venta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venta venta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _ventaService.ActualizarVentaAsync(id, venta);
                    if (!result)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar la venta con ID {Id}.", id);
                    ModelState.AddModelError("", "No se pudo guardar los cambios. Inténtalo de nuevo.");
                    return View(venta);
                }
            }
            return View(venta);
        }

        // GET: Venta/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
                if (venta == null)
                {
                    return NotFound();
                }
                return View(venta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la venta a eliminar con ID {Id}.", id);
                return View("Error");
            }
        }

        // POST: Venta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _ventaService.EliminarVentaAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la venta con ID {Id}.", id);
                return View("Error");
            }
        }



    }
}
