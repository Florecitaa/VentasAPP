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
        public ProductoController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _productoService.ObtenerTodosLosProductosAsync();
            return View(productos);
        }
        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
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
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.ActualizarProductoAsync(id, producto);
                return RedirectToAction("Index");
            }
            return View(producto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productoService.EliminarProductoAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportToPdf()
        {
            var productos = await _productoService.ObtenerTodosLosProductosAsync();

            using (var stream = new MemoryStream())
            {
                var doc = new iTextSharp.text.Document();
                PdfWriter.GetInstance(doc, stream).CloseStream = false;
                doc.Open();
                doc.Add(new Paragraph("Lista de Productos"));
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

                return File(stream.ToArray(), "application/pdf", "Productos.pdf");
            }
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var productos = await _productoService.ObtenerTodosLosProductosAsync();

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
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Productos.xlsx");
            }
        }
    }
}
