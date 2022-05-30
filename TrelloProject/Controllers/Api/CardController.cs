using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TrelloProject.Models;

namespace TrelloProject.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        [Route("GetAllCards")]
        public async Task<List<CardModel>> GetAllCards()
        {
            var response = await SendRequestToApi(null, "/GetItems", HttpMethod.Get);
            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CardModel>>(jsonString);
        }

        [Route("CreateCard")]
        public async void CreateCard(CardModel cardModel)
        {
            cardModel.OwnerEmail = HttpContext.Session.GetString("UserEmail");

            var response = await SendRequestToApi(
                JsonConvert.SerializeObject(cardModel),
                "/AddItem",
                HttpMethod.Post);
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
    }
}
