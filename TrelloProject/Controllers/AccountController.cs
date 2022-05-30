using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TrelloProject.Models;

namespace TrelloProject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("JWTtoken") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserModel userModel)
        {
            var response = await PostToApi(JsonConvert.SerializeObject(userModel), "/SignIn");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = (JObject)JsonConvert.DeserializeObject(responseContent);
                HttpContext.Session.SetString("JWTtoken", token["token"].Value<string>());
                HttpContext.Session.SetString("UserEmail", userModel.Email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Failed to login";
                return View("Index");
            }
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserModel userModel)
        {
            var response = await PostToApi(JsonConvert.SerializeObject(userModel), "/SignUp");

            if (response.IsSuccessStatusCode)
                ViewBag.Message = "Successfully registered";
            else
                ViewBag.Message = "Failed to register";

            return View("Index");
        }

        private async Task<HttpResponseMessage> PostToApi(string content, string uri)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://mybekonlineauth.azurewebsites.net/api" + uri),
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                return await client.SendAsync(request);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return View("Index");
        }
    }
}
