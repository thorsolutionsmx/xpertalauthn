using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using xpertalModels;
using xpertalwebapp.Models;


namespace xpertalwebapp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly IDownstreamApi _xpertalApi;        
        private readonly IHttpClientFactory _xpertalApi;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory downstreamApi)
        {
            _logger = logger;
            _xpertalApi = downstreamApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Salir()
        {
            return View();
        }



        public async Task<IActionResult> PedirAPI()
        {
            ViewBag.resultadoapi = await ClienteTradicional();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region"Codigo"

        private async Task<string> ClienteTradicional()
        {
            string Resultado = "No hay resultado";
            var httpRequestMessage = new HttpRequestMessage(
          HttpMethod.Get,
          "/WeatherForecast");
            var httpClient = _xpertalApi.CreateClient("ApiProtegida");
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