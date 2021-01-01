using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RaceVentura.RaceVenturaApiModels;
using Xamarin.Essentials;

namespace RaceVentura.Services
{
    public class RaceVenturaApiClient : IRaceVenturaApiClient
    {
        private readonly HttpClient _httpClient;
        private const string apiUrl = "https://raceventura.westeurope.cloudapp.azure.com/api/";
        private readonly string appApiUrl = $"{apiUrl}appapi";

        public RaceVenturaApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RegisterToRaceModel> RegisterToRace(Guid raceId, Guid teamId, Guid uniqueId)
        {
            Uri uri = new Uri($"{appApiUrl}/registertorace");

            var model = new RegisterToRaceModel
            {
                RaceId = raceId,
                TeamId = teamId,
                UniqueId = uniqueId
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<RegisterToRaceModel>(responseContent);
            }
            else
            {
                ProcessApiError(responseContent, "register to race");
            }

            return model;
        }

        public async Task<RegisterPointModel> RegisterPoint(Guid raceId, Guid uniqueId, Guid pointId, double latitude, double longitude, string answer)
        {
            Uri uri = new Uri($"{appApiUrl}/registerpoint");

            var model = new RegisterPointModel
            {
                RaceId = raceId,
                PointId = pointId,
                UniqueId = uniqueId,
                Latitude = latitude,
                Longitude = longitude,
                Question = null,
                Answer = answer.Trim()
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<RegisterPointModel>(responseContent);
            }
            else
            {
                ProcessApiError(responseContent, "register to race");
            }

            return model;
        }

        public async void GoToResultPage(Guid raceId)
        {
            await Browser.OpenAsync(new Uri($"{apiUrl}results/getraceresults?raceid={raceId}"), BrowserLaunchMode.SystemPreferred);
        }

        private static void ProcessApiError(string responseContent, string function)
        {
            ErrorCodes errorCode;
            if (Enum.TryParse(responseContent, out errorCode))
            {
                throw new RaceVenturaApiException($"Something went wrong in {function}.", errorCode);
            }

            throw new Exception("Could not parse error code.");
        }
    }
}
