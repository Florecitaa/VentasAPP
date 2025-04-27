namespace VentasAPP.Models
{
    public class DetalleVenta
    {
        public int IdDetalledeventa { get; set; }
        public int idproducto { get; set; }
        public int IdVenta { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public decimal PrecioUnitario { get; set; }


        public Producto Producto { get; set; }
        public Venta Venta { get; set; }
    }
}
