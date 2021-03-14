using System;
using RaceVentura.Models;
using RaceVentura.RaceVenturaApiModels;
using RaceVentura.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class NewRacePage : ContentPage
    {
        private readonly IParseQrCodeResultService _qrCodeResultParser;
        private readonly IRaceVenturaApiClient _raceVenturaApiClient;
        private readonly IDataStore<Race> _dataStore;

        public NewRacePage()
        {
            InitializeComponent();
            _qrCodeResultParser = DependencyService.Get<IParseQrCodeResultService>();
            _raceVenturaApiClient = DependencyService.Get<IRaceVenturaApiClient>();
            _dataStore = DependencyService.Get<IDataStore<Race>>();
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            QrScanner.IsScanning = false;
            await Navigation.PopModalAsync();
        }

        private async void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() => QrScanner.IsScanning = false);

                var qrCodeResult = _qrCodeResultParser.ParseResult(result.Text);

                if (qrCodeResult.QrCodeType != QrCodeType.RegisterToRace)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ShowPopupAndLeavePage("Error", "This is not a team registration QR code.");
                    });
                    return;
                }

                if (await _dataStore.GetItemAsync(qrCodeResult.RaceId) != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        ShowPopupAndLeavePage("Error", "You are already registered to this race.");
                    });
                    return;
                }

                var current = Connectivity.NetworkAccess;
                if (current != NetworkAccess.Internet)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        ShowPopupAndLeavePage("Error", "You need an active internet connection for this app to work. Please connect to the internet.");
                    });
                    return;
                }

                var uniqueId = Guid.NewGuid();
                var apiResponse = await _raceVenturaApiClient.RegisterToRace(qrCodeResult.RaceId, qrCodeResult.TeamId, uniqueId);

                var race = new Race
                {
                    RaceId = apiResponse.RaceId,
                    Name = apiResponse.Name,
                    UniqueId = apiResponse.UniqueId,
                    RaceActive = true
                };

                MessagingCenter.Send(this, "AddItem", race);

                Device.BeginInvokeOnMainThread(() => Navigation.PopModalAsync());
            }
            catch (RaceVenturaApiException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.MaxIdsReached:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ShowPopupAndLeavePage("Error", "You cannot register more devices to this team.");
                        });
                        break;

                    case ErrorCodes.RaceEnded:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ShowPopupAndLeavePage("Error", "The team you are trying to register to has finished the race, you cannot register to this team anymore.");
                        });
                        break;

                    default:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ShowPopupAndLeavePage("Error", "Something went wrong please try again.");
                        });
                        break;
                }
            }
            catch (Exception)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowPopupAndLeavePage("Error", "Something went wrong please try again.");
                });
            }
        }

        private void ShowPopupAndLeavePage(string title, string message)
        {
            DisplayAlert(title, message, "Ok");
            Navigation.PopModalAsync();
            QrScanner.IsScanning = false;
        }
    }
}
