using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://mybekonlineauth.azurewebsites.net/api/SignIn", content);

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
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> RegisterAsync(UserModel userModel)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://mybekonlineauth.azurewebsites.net/api/SignUp", content);

                if (response.IsSuccessStatusCode)
                    ViewBag.Message = "Successfully registered";
                else
                    ViewBag.Message = "Failed to register";

                return View("Index");
            }
        }
    }
}
