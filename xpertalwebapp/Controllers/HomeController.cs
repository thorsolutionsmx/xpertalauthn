using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;
using System.Text.Json;
using xpertaloxxomodels;
using xpertalwebapp.Models;

namespace xpertalwebapp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _misapi;
        private readonly IConfiguration _config;
        private readonly ITokenAcquisition _xptoken;

        public HomeController(ILogger<HomeController> logger, 
            IHttpClientFactory micliente, 
            IConfiguration config, 
            ITokenAcquisition xptoken)
        {
            _logger = logger;
            _misapi = micliente;
            _config = config;
            _xptoken = xptoken;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Authorize(Roles = "jefefinanzas")]
        public async Task< IActionResult> LlamaApi()
        {
            ViewBag.ResultadoApi = await ClienteTradicional();
            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region "codigo"
        private async Task< string> ClienteTradicional()
        {
            string Resultado = "No hay resultado";


            var scope = _config.GetValue<string>("AzureAd:Scopes");
            var accessToken = await _xptoken.GetAccessTokenForUserAsync(new[] { scope });


            var httpRequestMessage = new HttpRequestMessage(
          HttpMethod.Get,
          "/WeatherForecast");

            var httpClient = _misapi.CreateClient("ApiProtegida");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();
                var Weather = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
                Resultado = JsonSerializer.Serialize(Weather);
            }
            return Resultado;
        }


        #endregion

    }
}