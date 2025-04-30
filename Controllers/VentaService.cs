using Newtonsoft.Json;
using System.Text;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class VentaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public VentaService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }
        public async Task<IEnumerable<Venta>> ObtenerTodasLasVentasAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Venta");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Venta>>(content);
        }

        public async Task<Venta> ObtenerVentaPorIdAsync(int idVenta)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Venta/" + idVenta);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Venta>(content);
        }

        public async Task<int> InsertarVentaAsync(Venta venta)
        {
            var json = JsonConvert.SerializeObject(venta);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Venta", stringContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al insertar venta: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            // Deserializamos el objeto completo y devolvemos su ID
            var created = JsonConvert.DeserializeObject<Venta>(content);
            return created.IDVenta;
        }



        public async Task<bool> ActualizarVentaAsync(int idVenta, Venta venta)
        {
            var json = JsonConvert.SerializeObject(venta);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(_baseUrl + "/Venta/" + idVenta, stringContent);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<bool> EliminarVentaAsync(int idVenta)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + "/Venta/" + idVenta);
            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
