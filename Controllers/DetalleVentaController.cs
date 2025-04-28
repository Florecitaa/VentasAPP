using Microsoft.AspNetCore.Mvc;

namespace VentasAPP.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly DetalleVentaService _detalleventaService;
        public DetalleVentaController(DetalleVentaService detalleventaService)
        {
            _detalleventaService = detalleventaService;
        }


    }
}
