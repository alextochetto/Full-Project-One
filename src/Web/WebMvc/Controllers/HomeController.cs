using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            ViewData["Message"] = "Your application description page.";

            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Home/Index"
            }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public IActionResult Logout()
        {
            //for signout
            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "Home/Index"
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> RequestApi()
        {
            var client = _httpClientFactory.CreateClient("payment");

            var accessToken = HttpContext.GetTokenAsync("access_token").Result;
            var idToken = HttpContext.GetTokenAsync("id_token").Result;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("/api/payment");
            if (!response.IsSuccessStatusCode)
            {
                return View(nameof(Privacy));
            }

            var model = new PaymentApiViewModel
            {
                AccessToken = accessToken,
                IdToken = idToken,
                JsonResult = response.Content.ReadAsStringAsync().Result
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
