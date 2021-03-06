﻿using System;
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
        private const string webUrl = "https://raceventura.nl/";
        private const string apiUrl = "https://raceventura.westeurope.cloudapp.azure.com/api/";
        private readonly string appApiUrl = $"{apiUrl}appapi";

        public RaceVenturaApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RegisterToRaceModel> RegisterToRace(Guid raceId, Guid teamId, Guid uniqueId)
        {
            Uri uri = new($"{appApiUrl}/registertorace");

            var model = new RegisterToRaceModel
            {
                RaceId = raceId,
                TeamId = teamId,
                UniqueId = uniqueId
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new(json, Encoding.UTF8, "application/json");

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
            Uri uri = new($"{appApiUrl}/registerpoint");

            var model = new RegisterPointModel
            {
                RaceId = raceId,
                PointId = pointId,
                UniqueId = uniqueId,
                Latitude = latitude,
                Longitude = longitude,
                Answer = answer.Trim()
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<RegisterPointModel>(responseContent);
            }
            else
            {
                ProcessApiError(responseContent, "register point");
            }

            return model;
        }

        public async Task<RegisterStageEndModel> RegisterStageEnd(Guid raceId, Guid uniqueId, Guid stageId)
        {
            Uri uri = new($"{appApiUrl}/registerstageend");

            var model = new RegisterStageEndModel
            {
                RaceId = raceId,
                UniqueId = uniqueId,
                StageId = stageId
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<RegisterStageEndModel>(responseContent);
            }
            else
            {
                ProcessApiError(responseContent, "register stage end");
            }

            return model;
        }

        public async Task<RegisterRaceEndModel> RegisterRaceEnd(Guid raceId, Guid uniqueId)
        {
            Uri uri = new($"{appApiUrl}/registerraceend");

            var model = new RegisterRaceEndModel
            {
                RaceId = raceId,
                UniqueId = uniqueId
            };

            string json = JsonConvert.SerializeObject(model);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                model = JsonConvert.DeserializeObject<RegisterRaceEndModel>(responseContent);
            }
            else
            {
                ProcessApiError(responseContent, "register race end");
            }

            return model;
        }

        public async Task<string> GetAppLatestVersion()
        {
            Uri uri = new($"{apiUrl}version/getappversion");

            var response = await _httpClient.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();

            return responseContent;
        }

        public async void GoToResultPage(Guid raceId)
        {
            await Browser.OpenAsync(GetRaceResultUri(raceId), BrowserLaunchMode.SystemPreferred);
        }

        public async void ShareRaceResults(Guid raceId)
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Uri = GetRaceResultUri(raceId).ToString(),
                Title = "Race result URL"
            });
        }

        private Uri GetRaceResultUri(Guid raceId)
        {
            return new Uri($"{webUrl}results?raceid={raceId}");
        }

        private static void ProcessApiError(string responseContent, string function)
        {
            if (Enum.TryParse(responseContent, out ErrorCodes errorCode))
            {
                throw new RaceVenturaApiException($"Something went wrong in {function}.", errorCode);
            }

            throw new Exception("Could not parse error code.");
        }
    }
}
