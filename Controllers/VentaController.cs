using Microsoft.AspNetCore.Mvc;

namespace VentasAPP.Controllers
{
    public class VentaController : Controller
    {
        private readonly VentaService _ventaService;
        public VentaController(VentaService ventaService)
        {
            _ventaService = ventaService;
        }
    }
}
