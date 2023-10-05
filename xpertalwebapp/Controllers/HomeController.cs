using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using xpertalModels;
using xpertalwebapp.Models;
using static System.Net.WebRequestMethods;


namespace xpertalwebapp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;
        //private readonly IDownstreamApi _xpertalApi;
        private readonly IHttpClientFactory _xpertalApi;
        private ITokenAcquisition _tokenAcquisition;


        public HomeController(ILogger<HomeController> logger, IConfiguration config, IHttpClientFactory downstreamApi, ITokenAcquisition tokenad)
        {
            _logger = logger;
            _xpertalApi = downstreamApi;
            _config = config;
            _tokenAcquisition = tokenad;
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
            ViewBag.resultadoapi = await ClienteSeguridad();
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region"Codigo"

        //private async Task<string> ClienteTradicional()
        //{
        //    string Resultado = "No hay resultado";
        //    var httpRequestMessage = new HttpRequestMessage(
        //  HttpMethod.Get,
        //  "/WeatherForecast");
        //    var httpClient = _xpertalApi.CreateClient("ApiProtegida");
        //    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        //    if (httpResponseMessage.IsSuccessStatusCode)
        //    {
        //        using var contentStream =
        //            await httpResponseMessage.Content.ReadAsStreamAsync();
        //        var Weather = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
        //        Resultado = JsonSerializer.Serialize(Weather);
        //    }
        //    return Resultado;
        //}

        private async Task<string> ClienteSeguridad()
        {
            string Resultado = "No hay resultado";
            var scope = _config.GetSection("AzureAd:Scopes").Get<IEnumerable<string>>();
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync( scope );
            var httpRequestMessage = new HttpRequestMessage(
          HttpMethod.Get,
          "/WeatherForecast");
            var httpClient = _xpertalApi.CreateClient("ApiProtegida");
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