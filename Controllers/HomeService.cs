using VentasAPP.Models;

namespace VentasAPP.Controllers
{
    public class HomeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public HomeService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<bool> ValidarUsuarioAsync(LoginViewModel credenciales)
        {
            
            var url = $"{_baseUrl}/Usuario/validar?correo={Uri.EscapeDataString(credenciales.Correo)}&clave={Uri.EscapeDataString(credenciales.Clave)}";
            var response = await _httpClient.PostAsync(url, null);
            
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
           
            return response.IsSuccessStatusCode;
        }




    }
}
