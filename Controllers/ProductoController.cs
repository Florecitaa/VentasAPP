using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VentasAPP.Models;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;

namespace VentasAPP.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ProductoService _productoService;
        private readonly ILogger<ProductoController> _logger;
        public ProductoController(ProductoService productoService, ILogger<ProductoController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }



        public async Task<IActionResult> Index(string nombre, decimal? precioMin, decimal? precioMax)
        {
            try
            {
                var productos = await _productoService.ObtenerTodosLosProductosAsync();

                
                ViewBag.Nombre = nombre;
                ViewBag.PrecioMin = precioMin;
                ViewBag.PrecioMax = precioMax;

                // Filtro con LINQ
                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
                }
                if (precioMin.HasValue)
                {
                    productos = productos.Where(p => p.Precio >= precioMin.Value);
                }
                if (precioMax.HasValue)
                {
                    productos = productos.Where(p => p.Precio <= precioMax.Value);
                }

                return View(productos.ToList());
            }
            catch (Exception ex)
            {
                
                return View("Error");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorIdAsync(id);
                if (producto == null) return NotFound();
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener producto con ID {Id}", id);
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.CrearProductoAsync(producto);
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorIdAsync(id);
                if (producto == null) return NotFound();
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar edición de producto {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (!ModelState.IsValid) return View(producto);

            try
            {
                await _productoService.ActualizarProductoAsync(id, producto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar producto {Id}", id);
                ModelState.AddModelError("", "No se pudo actualizar el producto.");
                return View(producto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorIdAsync(id);
                if (producto == null) return NotFound();
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar eliminación de producto {Id}", id);
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.EliminarProductoAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar producto {Id}", id);
                return View("Error");
            }
        }
        public async Task<IActionResult> ExportToPdf(string nombre, decimal? precioMin, decimal? precioMax)
        {
            var productos = await _productoService.ObtenerTodosLosProductosAsync();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }
            if (precioMin.HasValue)
            {
                productos = productos.Where(p => p.Precio >= precioMin.Value);
            }
            if (precioMax.HasValue)
            {
                productos = productos.Where(p => p.Precio <= precioMax.Value);
            }

            using (var stream = new MemoryStream())
            {
                var doc = new Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();
                doc.Add(new Paragraph("Lista de Productos Filtrados"));
                doc.Add(new Paragraph(" "));

                var table = new PdfPTable(5);
                table.AddCell("Nombre");
                table.AddCell("Descripción");
                table.AddCell("Precio");
                table.AddCell("Categoría");
                table.AddCell("Stock");

                foreach (var producto in productos)
                {
                    table.AddCell(producto.Nombre);
                    table.AddCell(producto.Detalle);
                    table.AddCell(producto.Precio.ToString());
                    table.AddCell(producto.TipoProducto);
                    table.AddCell(producto.Disponible.ToString());
                }

                doc.Add(table);
                doc.Close();
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", "ProductosFiltrados.pdf");
            }
        }


        public async Task<IActionResult> ExportToExcel(string nombre, decimal? precioMin, decimal? precioMax)
        {
            var productos = await _productoService.ObtenerTodosLosProductosAsync();

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
            }
            if (precioMin.HasValue)
            {
                productos = productos.Where(p => p.Precio >= precioMin.Value);
            }
            if (precioMax.HasValue)
            {
                productos = productos.Where(p => p.Precio <= precioMax.Value);
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Productos");

                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Descripción";
                worksheet.Cells[1, 3].Value = "Precio";
                worksheet.Cells[1, 4].Value = "Categoría";
                worksheet.Cells[1, 5].Value = "Stock";

                int row = 2;
                foreach (var producto in productos)
                {
                    worksheet.Cells[row, 1].Value = producto.Nombre;
                    worksheet.Cells[row, 2].Value = producto.Detalle;
                    worksheet.Cells[row, 3].Value = producto.Precio;
                    worksheet.Cells[row, 4].Value = producto.TipoProducto;
                    worksheet.Cells[row, 5].Value = producto.Disponible;
                    row++;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProductosFiltrados.xlsx");
            }
        }


        private List<ItemCarrito> ObtenerCarrito()
        {
            var carrito = HttpContext.Session.GetString("Carrito");
            if (carrito != null)
                return JsonConvert.DeserializeObject<List<ItemCarrito>>(carrito);
            return new List<ItemCarrito>();
        }

        private void GuardarCarrito(List<ItemCarrito> carrito)
        {
            HttpContext.Session.SetString("Carrito", JsonConvert.SerializeObject(carrito));
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null) return NotFound();

            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(p => p.ProductoId == id);
            if (item != null)
                item.Cantidad++;
            else
                carrito.Add(new ItemCarrito { ProductoId = id, Nombre = producto.Nombre, Precio = producto.Precio, Cantidad = 1 });

            GuardarCarrito(carrito);
            return RedirectToAction("Index");
        }

        public IActionResult VerCarrito()
        {
            var carrito = ObtenerCarrito();
            ViewBag.Total = carrito.Sum(x => x.Precio * x.Cantidad);
            return View(carrito);
        }


    }
}
