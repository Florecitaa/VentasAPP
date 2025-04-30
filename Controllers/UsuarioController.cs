using Microsoft.AspNetCore.Mvc;
using VentasAPP.Models;
using Microsoft.Extensions.Logging;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Newtonsoft.Json;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;

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
        public async Task<IActionResult> Index(string nombre, string rol)
        {
            try
            {
                var usuarios = await _usuarioService.ObtenerUsuariosAsync();

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    usuarios = usuarios.Where(u => u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(rol))
                {
                    usuarios = usuarios.Where(u => u.Rol.Equals(rol, StringComparison.OrdinalIgnoreCase));
                }

                ViewBag.Nombre = nombre;
                ViewBag.Rol = rol;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            ModelState.Remove("Rol");

            if (ModelState.IsValid)
            {
                try
                {
                    // Aqui se establece que al registrar un nuevo usuario sera un cliente
                    usuario.Rol = "Cliente";
                    await _usuarioService.CrearUsuarioAsync(usuario);

                    // y nos envia a productos 
                    return RedirectToAction("Index", "Home");

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

        [HttpPost]
        public async Task<IActionResult> PostUsuario(Usuario usuario)
        {
            try
            {
                int newUserId = await _usuarioService.CrearUsuarioAsync(usuario);
               
                TempData["MensajeExito"] = "Usuario creado exitosamente. Por favor, inicie sesión.";
                return RedirectToAction("Index", "Home"); 
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", "Ocurrió un error al crear el usuario: " + ex.Message);
                return View(usuario); 
            }
        }

        public async Task<IActionResult> ExportToExcel(string nombre, string rol)
        {
            var usuarios = await _usuarioService.ObtenerUsuariosAsync();

            if (!string.IsNullOrWhiteSpace(nombre))
                usuarios = usuarios.Where(u => u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(rol))
                usuarios = usuarios.Where(u => u.Rol.Equals(rol, StringComparison.OrdinalIgnoreCase));

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Usuarios");

            worksheet.Cells[1, 1].Value = "Nombre";
            worksheet.Cells[1, 2].Value = "Apellido";
            worksheet.Cells[1, 3].Value = "Correo";
            worksheet.Cells[1, 4].Value = "Rol";

            int row = 2;
            foreach (var u in usuarios)
            {
                worksheet.Cells[row, 1].Value = u.Nombre;
                worksheet.Cells[row, 2].Value = u.Apellido;
                worksheet.Cells[row, 3].Value = u.Correo;
                worksheet.Cells[row, 4].Value = u.Rol;
                row++;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Usuarios.xlsx");
        }


        public async Task<IActionResult> ExportToPdf(string nombre, string rol)
        {
            var usuarios = await _usuarioService.ObtenerUsuariosAsync();

            if (!string.IsNullOrWhiteSpace(nombre))
                usuarios = usuarios.Where(u => u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(rol))
                usuarios = usuarios.Where(u => u.Rol.Equals(rol, StringComparison.OrdinalIgnoreCase));

            using var stream = new MemoryStream();
            var doc = new Document();
            PdfWriter.GetInstance(doc, stream).CloseStream = false;
            doc.Open();

            var table = new PdfPTable(4); 
            table.AddCell("Nombre");
            table.AddCell("Apellido");
            table.AddCell("Correo");
            table.AddCell("Rol");

            foreach (var u in usuarios)
            {
                table.AddCell(u.Nombre);
                table.AddCell(u.Apellido);
                table.AddCell(u.Correo);
                table.AddCell(u.Rol);
            }

            doc.Add(table);
            doc.Close();

            stream.Position = 0;
            return File(stream.ToArray(), "application/pdf", "Usuarios.pdf");
        }

    }
}

