using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
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
        private readonly IDownstreamApi _xpertalApi;
        //private readonly IHttpClientFactory _xpertalApi;
        private ITokenAcquisition _tokenAcquisition;


        public HomeController(ILogger<HomeController> logger, IConfiguration config, IDownstreamApi downstreamApi, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _xpertalApi = downstreamApi;
            _config = config;
            _tokenAcquisition = tokenAcquisition;
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

        [Authorize(Roles = "finanzas,valuadoresgrant")]
        public async Task<IActionResult> PedirAPI()
        {
            ViewBag.resultadoapi = await ClienteDownStream();
            return View();
        }
        public IActionResult MostrarToken()
        {
            int _contador = 1;
            Dictionary<string, string> _losclaims = new Dictionary<string, string>();
            foreach (var token in User.Claims)
            {
                if (!_losclaims.Keys.Contains(token.Type))
                    _losclaims.Add(token.Type, token.Value);
                else
                {
                    _losclaims.Add(token.Type + _contador.ToString(), token.Value);
                    _contador++;
                }
            }
            return View(_losclaims);
        }

        [Authorize(policy:"numempleado")]
        public async Task<IActionResult> MostrarAccess()
        {

            var scope = _config.GetSection("AzureAd:Scopes").Get<IEnumerable<string>>();
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scope);
if (accessToken == null)
                return View(new Dictionary<string, string>());

            var stream = accessToken;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            int _contador = 1;


            Dictionary<string, string> _losclaims = new Dictionary<string, string>();
            foreach (var token in tokenS.Claims)
            {
                if (!_losclaims.Keys.Contains(token.Type))
                    _losclaims.Add(token.Type, token.Value);
                else
                {
                    _losclaims.Add(token.Type + _contador.ToString(), token.Value);
                    _contador++;
                }
            }
            return View(_losclaims);
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

        //private async Task<string> ClienteSeguridad()
        //{
        //    string Resultado = "No hay resultado";
        //    var scope = _config.GetSection("AzureAd:Scopes").Get<IEnumerable<string>>();
        //    var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync( scope );
        //    var httpRequestMessage = new HttpRequestMessage(
        //  HttpMethod.Get,
        //  "/WeatherForecast");
        //    var httpClient = _xpertalApi.CreateClient("ApiProtegida");
        //    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        //    var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        //    if (httpResponseMessage.IsSuccessStatusCode)
        //    {
        //        using var contentStream =
        //            await httpResponseMessage.Content.ReadAsStreamAsync();
        //        var Weather = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
        //        Resultado = JsonSerializer.Serialize(Weather);
        //    }
        //        else
        //        {
        //            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //            Resultado = $"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}";
        //};
        //    return Resultado;
        //}


        private async Task<string> ClienteDownStream()
        {
            string Resultado = "No hay resultado";
            using var response = await _xpertalApi.CallApiForUserAsync("ApiProtegida", o => o.RelativePath= "/WeatherForecast").ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using var contentStream =
                    await response.Content.ReadAsStreamAsync();
                var Weather = await JsonSerializer.DeserializeAsync<IEnumerable<WeatherForecast>>(contentStream);
                Resultado = JsonSerializer.Serialize(Weather);
                //var apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                //ViewData["ApiResult"] = apiResult;
            }
            else
            {
                var error = response.ToString();
                Resultado = $"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {error}";
            };
            return Resultado;
        }

        #endregion

    }
}