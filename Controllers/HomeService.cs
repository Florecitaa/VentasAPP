using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class HomeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly ILogger<HomeService> _logger;
        public HomeService(HttpClient httpClient, IConfiguration configuration, ILogger<HomeService> logger) //Injeccion de dependencias
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
            _logger = logger;
        }

        public async Task<bool> ValidarUsuarioAsync(LoginViewModel credenciales)
        {
            try
            {
                var url = $"{_baseUrl}/Usuario/validar?correo={Uri.EscapeDataString(credenciales.Correo)}&clave={Uri.EscapeDataString(credenciales.Clave)}";
                var response = await _httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error al validar usuario. Status Code: {response.StatusCode}, Body: {responseBody}");
                    return false; 
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción al validar usuario: {ex.Message}");
                return false; 
            }
        }




    }
}
