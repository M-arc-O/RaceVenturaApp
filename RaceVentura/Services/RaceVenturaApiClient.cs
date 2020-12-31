using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RaceVentura.RaceVenturaApiModels;

namespace RaceVentura.Services
{
    public class RaceVenturaApiClient: IRaceVenturaApiClient
    {
        private readonly HttpClient _httpClient;
        private const string apiUrl = "https://raceventura.westeurope.cloudapp.azure.com/api/appapi";

        public RaceVenturaApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RegisterToRaceModel> RegisterToRace(Guid raceId, Guid teamId, Guid uniqueId)
        {
            Uri uri = new Uri($"{apiUrl}/registertorace");

            var model = new RegisterToRaceModel
            {
                RaceId = raceId,
                TeamId = teamId,
                UniqueId = uniqueId
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            { 
                var responseContent = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<RegisterToRaceModel>(responseContent);
            }
            else
            {

            }

            return model;
        }
    }
}
