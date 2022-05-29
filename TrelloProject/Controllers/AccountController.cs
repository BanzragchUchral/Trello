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

                response.EnsureSuccessStatusCode();

                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var token = (JObject)JsonConvert.DeserializeObject(responseContent);
                    HttpContext.Session.SetString("JWTtoken", token["token"].Value<string>());

                    return RedirectToAction("Index", "Home");
                }
            }

            return View("Index");
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

                return RedirectToAction("Index");
            }
        }
    }
}
