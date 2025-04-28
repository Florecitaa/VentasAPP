using Newtonsoft.Json;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class DetalleVentaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public DetalleVentaService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }
        public async Task<IEnumerable<DetalleVenta>> ObtenerTodosLosDetallesDeVentaAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/DetalleVenta");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<DetalleVenta>>(content);
        }

        public async Task<DetalleVenta> ObtenerDetalleVentaPorIdAsync(int idDetalleVenta)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/DetalleVenta/" + idDetalleVenta);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DetalleVenta>(content);
        }

        public async Task CrearDetalleVentaAsync(DetalleVenta detalleVenta)
        {
            var json = JsonConvert.SerializeObject(detalleVenta);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/DetalleVenta", stringContent);
            response.EnsureSuccessStatusCode();
          
        }

        public async Task<bool> ActualizarDetalleVentaAsync(int idDetalleVenta, DetalleVenta detalleVenta)
        {
            var json = JsonConvert.SerializeObject(detalleVenta);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(_baseUrl + "/DetalleVenta/" + idDetalleVenta, stringContent);
            response.EnsureSuccessStatusCode();
            return true; 
        }

        public async Task<bool> EliminarDetalleVentaAsync(int idDetalleVenta)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + "/DetalleVenta/" + idDetalleVenta);
            response.EnsureSuccessStatusCode();
            return true; 
        }
    }
}
