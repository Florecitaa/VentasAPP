namespace VentasAPP.Models
{
    public class Producto
    {
        public int idproducto { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public decimal Precio { get; set; }
        public int Disponible { get; set; }
        public string TipoProducto { get; set; }
    }
}
