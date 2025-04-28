using Newtonsoft.Json;
using System.Data.SqlClient;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class ProductoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public ProductoService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }


        public async Task<IEnumerable<Producto>> ObtenerTodosLosProductosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Producto");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Producto>>(content);
        }

        public async Task<Producto> ObtenerProductoPorIdAsync(int idProducto)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Producto/" + idProducto);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Producto>(content);
        }

        public async Task CrearProductoAsync(Producto producto)
        {
            var json = JsonConvert.SerializeObject(producto);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Producto", stringContent);
            response.EnsureSuccessStatusCode();
           
        }

        public async Task<bool> ActualizarProductoAsync(int idProducto, Producto producto)
        {
            var json = JsonConvert.SerializeObject(producto);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(_baseUrl + "/Producto/" + idProducto, stringContent);
            response.EnsureSuccessStatusCode();
            return true; 
        }

        public async Task<bool> EliminarProductoAsync(int idProducto)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + "/Producto/" + idProducto);
            response.EnsureSuccessStatusCode();
            return true;
        }

    }
}

