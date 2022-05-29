using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
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
                //client.BaseAddress = new Uri("https://mybekonlineauth.azurewebsites.net");

                //var postTask = client.PostAsJsonAsync<UserModel>("/api/SignIn", userModel);

                //postTask.Wait();

                //if (postTask.Result.IsSuccessStatusCode)
                //{
                //    return RedirectToAction("Index", "Home");
                //}

                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var values = new Dictionary<string, string>
                {
                    { "Email", "test@test.com" },
                    { "Password", "Qwerty12345" }
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.ExpectContinue = false;

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
    }
}
