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
            if (!await IsAuthenticatedAsync())
                return RedirectToAction("Index", "Account");

            var model = await GetAllCards();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CardCreate");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CardModel cardModel)
        {
            cardModel.OwnerEmail = HttpContext.Session.GetString("UserEmail");

            var response = await SendRequestToApi(
                JsonConvert.SerializeObject(cardModel),
                "/AddItem",
                HttpMethod.Post);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAsync(string id)
        {
            var model = await GetAllCards();

            return PartialView("_CardDetail", model.Where(x => x.id == id).FirstOrDefault());
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

            var response = await SendRequestToApi(
                JsonConvert.SerializeObject(model),
                "/UpdateItem",
                HttpMethod.Post);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(CardModel model)
        {
            var response = await SendRequestToApi(
                JsonConvert.SerializeObject(model),
                "/UpdateItem",
                HttpMethod.Post);

            return View("Index", await GetAllCards());
        }

        public async Task<IActionResult> DeleteAsync(string id)
        {
            var values = new
            {
                id = id
            };

            var response = await SendRequestToApi(
                JsonConvert.SerializeObject(values),
                "/DeleteItem",
                HttpMethod.Delete);

            return RedirectToAction("Index", "Home");
        }

        private async Task<HttpResponseMessage> SendRequestToApi(string content, string uri, HttpMethod httpMethod)
        {
            var jwtToken = HttpContext.Session.GetString("JWTtoken");

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    RequestUri = new Uri("https://mybekonlineauth.azurewebsites.net/api" + uri),
                    Content = (content == null) ? null : new StringContent(content, Encoding.UTF8, "application/json")
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                return await client.SendAsync(request);
            }
        }

        private async Task<bool> IsAuthenticatedAsync()
        {
            var response = await SendRequestToApi(null, "/AuthTest", HttpMethod.Get);

            return response.IsSuccessStatusCode;
        }

        private async Task<List<CardModel>> GetAllCards()
        {
            var response = await SendRequestToApi(null, "/GetItems", HttpMethod.Get);
            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CardModel>>(jsonString);
        }
    }
}
