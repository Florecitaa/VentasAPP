using Microsoft.AspNetCore.Mvc;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly DetalleVentaService _detalleVentaService;
        private readonly ILogger<DetalleVentaController> _logger;

        // Constructor que accede a la lógica y el logger para registrar errores
        public DetalleVentaController(DetalleVentaService detalleVentaService, ILogger<DetalleVentaController> logger)
        {
            _detalleVentaService = detalleVentaService;
            _logger = logger;
        }

        // GET: DetalleVenta
        // Muestra la lista completa de detalles de venta
        public async Task<IActionResult> Index()
        {
            try
            {
                var detallesVenta = await _detalleVentaService.ObtenerTodosLosDetallesDeVentaAsync();
                return View(detallesVenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de detalles de venta.");
                return View("Error"); 
            }
        }

        // GET: DetalleVenta/Details/5
        // Muestra los datos de un detalle de venta específico
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var detalleVenta = await _detalleVentaService.ObtenerDetalleVentaPorIdAsync(id);
                if (detalleVenta == null)
                {
                    return NotFound();
                }
                return View(detalleVenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles de la venta con ID {Id}.", id);
                return View("Error");
            }
        }

        // GET: DetalleVenta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DetalleVenta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleVenta detalleVenta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _detalleVentaService.CrearDetalleVentaAsync(detalleVenta);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear un nuevo detalle de venta.");
                    ModelState.AddModelError("", "No se pudo crear el detalle de venta. Inténtalo de nuevo.");
                    return View(detalleVenta);
                }
            }
            return View(detalleVenta);
        }

        // GET: DetalleVenta/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detalleVenta = await _detalleVentaService.ObtenerDetalleVentaPorIdAsync(id);
                if (detalleVenta == null)
                {
                    return NotFound();
                }
                return View(detalleVenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el detalle de venta a editar con ID {Id}.", id);
                return View("Error");
            }

        }

        // POST: DetalleVenta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleVenta detalleVenta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _detalleVentaService.ActualizarDetalleVentaAsync(id, detalleVenta);
                    if (!result)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar el detalle de venta con ID {Id}.", id);
                    ModelState.AddModelError("", "No se pudo guardar los cambios. Inténtalo de nuevo.");
                    return View(detalleVenta);
                }
            }
            return View(detalleVenta);
        }

        // GET: DetalleVenta/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var detalleVenta = await _detalleVentaService.ObtenerDetalleVentaPorIdAsync(id);
                if (detalleVenta == null)
                {
                    return NotFound();
                }
                return View(detalleVenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el detalle de venta a eliminar con ID {Id}.", id);
                return View("Error");
            }
        }

        // POST: DetalleVenta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _detalleVentaService.EliminarDetalleVentaAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el detalle de venta con ID {Id}.", id);
                return View("Error");
            }
        }

    }
}
