namespace VentasAPP.Models
{
    public class Venta
    {
     
        public int IdUsuario { get; set; }
        public decimal monto_total { get; set; }
        public string metodo_pago { get; set; }
        public int IDVenta { get; internal set; }
        public DateTime Fecha { get; set; }
        
    }
}
