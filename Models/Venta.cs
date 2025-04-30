namespace VentasAPP.Models
{
    public class Venta
    {
        //public int IDVenta { get; set; }
        //public int IdUsuario { get; set; }
        //public DateTime Fecha { get; set; }
        //public decimal MontoTotal { get; set; }
        //public string MetodoPago { get; set; }



        public int IdUsuario { get; set; }
        public decimal monto_total { get; set; }
        public string metodo_pago { get; set; }
        public int IDVenta { get; internal set; }
        public DateTime Fecha { get; set; }
        //public Venta()
        //{
        //    Fecha = DateTime.Now;
        //}
    }
}
