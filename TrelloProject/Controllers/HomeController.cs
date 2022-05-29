using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TrelloProject.Constants;
using TrelloProject.Models;

namespace TrelloProject.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");
            if (jwtToken == null)
                return RedirectToAction("Index", "Account");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await client.GetAsync("https://mybekonlineauth.azurewebsites.net/api/GetItems");
                var jsonString = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<List<CardModel>>(jsonString);

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CardCreate");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CardModel cardModel)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var content = new StringContent(
                    JsonConvert.SerializeObject(cardModel), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://mybekonlineauth.azurewebsites.net/api/AddItem", content);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await client.GetAsync("https://mybekonlineauth.azurewebsites.net/api/GetItems");
                var jsonString = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<List<CardModel>>(jsonString);

                return PartialView("_CardDetail", model.Where(x => x.id == id).FirstOrDefault());
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveAsync(string id, string status)
        {
            var statusValue = (Status)Enum.Parse(typeof(Status), status);

            var model = new
            {
                id = id,
                Status = ((int)statusValue).ToString()
            };

            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://mybekonlineauth.azurewebsites.net/api/UpdateItem"),
                    Content = new StringContent(
                        JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await client.SendAsync(request);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(CardModel model)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var content = new StringContent(
                    JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://mybekonlineauth.azurewebsites.net/api/UpdateItem",
                    content);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {

                var values = new
                {
                    id = id
                };

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri("https://mybekonlineauth.azurewebsites.net/api/DeleteItem"),
                    Content = new StringContent(
                        JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await client.SendAsync(request);

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
