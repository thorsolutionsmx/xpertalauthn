using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory micliente)
        {
            _logger = logger;
            _misapi = micliente;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



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

            var httpRequestMessage = new HttpRequestMessage(
          HttpMethod.Get,
          "/WeatherForecast");

            var httpClient = _misapi.CreateClient("ApiProtegida");
         
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