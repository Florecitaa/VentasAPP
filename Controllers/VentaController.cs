using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
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
        public async Task<IActionResult> Index(DateTime? fechaDesde,DateTime? fechaHasta,decimal? totalMin,decimal? totalMax, int? userId)
        {
            try
            {
                // 1) Obtén todas las ventas
                var ventas = await _ventaService.ObtenerTodasLasVentasAsync();

                // 2) Guarda los valores en ViewBag para que la vista los muestre en los inputs
                ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
                ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
                ViewBag.TotalMin = totalMin;
                ViewBag.TotalMax = totalMax;
                ViewBag.UserId = userId;

                // 3) Aplica los filtros si vinieron
                if (fechaDesde.HasValue)
                    ventas = ventas.Where(v => v.Fecha.Date >= fechaDesde.Value.Date);
                if (fechaHasta.HasValue)
                    ventas = ventas.Where(v => v.Fecha.Date <= fechaHasta.Value.Date);
                if (totalMin.HasValue)
                    ventas = ventas.Where(v => v.monto_total >= totalMin.Value);
                if (totalMax.HasValue)
                    ventas = ventas.Where(v => v.monto_total <= totalMax.Value);
                if (userId.HasValue)
                    ventas = ventas.Where(v => v.IdUsuario == userId.Value);

                return View(ventas.ToList());
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

        [HttpPost]
        public async Task<IActionResult> ConfirmarCompra()
        {
            var carritoStr = HttpContext.Session.GetString("Carrito");
            if (string.IsNullOrEmpty(carritoStr))
                return RedirectToAction("VerCarrito", "Producto");

            var carrito = JsonConvert.DeserializeObject<List<ItemCarrito>>(carritoStr);
            var total = carrito.Sum(x => x.Precio * x.Cantidad);

            var usuarioIdClaim = User.FindFirst("IDUsuario");
            if (usuarioIdClaim == null)
                return RedirectToAction("Login", "Home");

            int usuarioId = int.Parse(usuarioIdClaim.Value);

            var venta = new Venta
            {
                IdUsuario = usuarioId,
                monto_total = total,
                metodo_pago = "Efectivo" 
            };

            await _ventaService.InsertarVentaAsync(venta);

            HttpContext.Session.Remove("Carrito");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        decimal? totalMin,
        decimal? totalMax,
        int? userId)
        {
            // Obtén y filtra igual que en Index
            var ventas = await _ventaService.ObtenerTodasLasVentasAsync();
            if (fechaDesde.HasValue) ventas = ventas.Where(v => v.Fecha.Date >= fechaDesde.Value.Date);
            if (fechaHasta.HasValue) ventas = ventas.Where(v => v.Fecha.Date <= fechaHasta.Value.Date);
            if (totalMin.HasValue) ventas = ventas.Where(v => v.monto_total >= totalMin.Value);
            if (totalMax.HasValue) ventas = ventas.Where(v => v.monto_total <= totalMax.Value);
            if (userId.HasValue) ventas = ventas.Where(v => v.IdUsuario == userId.Value);

            using var stream = new MemoryStream();
            var doc = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(doc, stream).CloseStream = false;
            doc.Open();

            var table = new PdfPTable(5) { WidthPercentage = 100 };
            table.AddCell("ID");
            table.AddCell("Fecha");
            table.AddCell("Total");
            table.AddCell("ID Usuario");
            table.AddCell("Método");

            foreach (var v in ventas)
            {
                table.AddCell(v.IDVenta.ToString());
                table.AddCell(v.Fecha.ToString("yyyy-MM-dd"));
                table.AddCell(v.monto_total.ToString("F2"));
                table.AddCell(v.IdUsuario.ToString());
                table.AddCell(v.metodo_pago);
            }

            doc.Add(new Paragraph("Lista de Ventas"));
            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream.ToArray(), "application/pdf", "VentasFiltradas.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            decimal? totalMin,
            decimal? totalMax,
            int? userId)
        {
            // Mismo filtrado
            var ventas = await _ventaService.ObtenerTodasLasVentasAsync();
            if (fechaDesde.HasValue) ventas = ventas.Where(v => v.Fecha.Date >= fechaDesde.Value.Date);
            if (fechaHasta.HasValue) ventas = ventas.Where(v => v.Fecha.Date <= fechaHasta.Value.Date);
            if (totalMin.HasValue) ventas = ventas.Where(v => v.monto_total >= totalMin.Value);
            if (totalMax.HasValue) ventas = ventas.Where(v => v.monto_total <= totalMax.Value);
            if (userId.HasValue) ventas = ventas.Where(v => v.IdUsuario == userId.Value);

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Ventas");
            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Fecha";
            ws.Cells[1, 3].Value = "Total";
            ws.Cells[1, 4].Value = "ID Usuario";
            ws.Cells[1, 5].Value = "Método";

            int row = 2;
            foreach (var v in ventas)
            {
                ws.Cells[row, 1].Value = v.IDVenta;
                ws.Cells[row, 2].Value = v.Fecha.ToString("yyyy-MM-dd");
                ws.Cells[row, 3].Value = v.monto_total;
                ws.Cells[row, 4].Value = v.IdUsuario;
                ws.Cells[row, 5].Value = v.metodo_pago;
                row++;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "VentasFiltradas.xlsx"
            );
        }



    }
}
