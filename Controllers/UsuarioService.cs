using Newtonsoft.Json;
using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public UsuarioService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }
        public async Task<IEnumerable<Usuario>> ObtenerUsuariosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Usuario");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
        }

        public async Task<Usuario> ObtenerUsuarioPorIdAsync(int id)
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Usuario/" + id);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Usuario>(content);
        }

        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            var json = JsonConvert.SerializeObject(usuario);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Usuario", stringContent);
            response.EnsureSuccessStatusCode();
            
        }

        public async Task<bool> ActualizarUsuarioAsync(int id, Usuario usuario)
        {
            var json = JsonConvert.SerializeObject(usuario);
            var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(_baseUrl + "/Usuario/" + id, stringContent);
            response.EnsureSuccessStatusCode();
            return true; 
        }

        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + "/Usuario/" + id);
            response.EnsureSuccessStatusCode();
            return true; 
        }

        public async Task<Usuario> ValidarUsuarioAsync(string correo, string clave)
        {
            
            var url = $"{_baseUrl}/Usuario/validar?correo={Uri.EscapeDataString(correo)}&clave={Uri.EscapeDataString(clave)}";
            var response = await _httpClient.PostAsync(url, null); 
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Usuario>(content);
        }
    }
}
