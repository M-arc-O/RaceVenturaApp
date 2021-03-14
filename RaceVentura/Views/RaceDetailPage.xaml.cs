using System;
using System.Threading.Tasks;
using RaceVentura.Models;
using RaceVentura.RaceVenturaApiModels;
using RaceVentura.Services;
using RaceVentura.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class RaceDetailPage : ContentPage
    {
        private readonly IParseQrCodeResultService _qrCodeResultParser;
        private readonly IRaceVenturaApiClient _raceVenturaApiClient;
        private readonly ILocationService _locationService;

        private readonly RaceDetailViewModel viewModel;

        public RaceDetailPage(RaceDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            _qrCodeResultParser = DependencyService.Get<IParseQrCodeResultService>();
            _raceVenturaApiClient = DependencyService.Get<IRaceVenturaApiClient>();
            _locationService = DependencyService.Get<ILocationService>();
        }

        private async void ScanQrCodeButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (current != NetworkAccess.Internet)
                {
                    await DisplayAlert("Error", "You need an active internet connection for this app to work. Please connect to the internet.", "Ok");
                    return;
                }

                viewModel.NotProcessing = false;
                ScanQrCodeButton.Text = "Scanning";

                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                scanner.TopText = "Scan a QR code";
                scanner.FlashButtonText = "Enable flashlight";
                scanner.CancelButtonText = "Cancel";
                var result = await scanner.Scan();

                ScanQrCodeButton.Text = "Processing";

                if (result != null)
                {
                    var parsedResult = _qrCodeResultParser.ParseResult(result.Text);

                    switch (parsedResult.QrCodeType)
                    {
                        case QrCodeType.RegisterPoint:
                            await HandleRegisterPoint(parsedResult);
                            break;

                        case QrCodeType.RegisterStageEnd:
                            await HandleRegisterStageEnd(parsedResult);
                            break;

                        case QrCodeType.RegisterRaceEnd:
                            await HandleRegisterRaceEnd(parsedResult);
                            break;

                        case QrCodeType.RegisterToRace:
                            await DisplayAlert("Error", "This is a race registration QR code. If you want to register to a race use the add race button in the main screen.", "Ok");
                            break;

                        default:
                            await DisplayAlert("Error", "Unknown QR code type.", "Ok");
                            break;
                    }
                }                
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Something went wrong while scanning the QR code.", "Ok");
                });
            }
            finally
            {
                viewModel.NotProcessing = true;
                ScanQrCodeButton.Text = "Scan QR code";
            }
        }

        private async Task HandleRegisterPoint(QrCodeResult parsedResult)
        {
            var answer = string.Empty;
            try
            {
                var location = await _locationService.GetLocation();

                var response = await _raceVenturaApiClient.RegisterPoint(parsedResult.RaceId, viewModel.Item.UniqueId, parsedResult.PointId, location.Latitude, location.Longitude, string.Empty);

                if (response.Type == PointTypeViewModel.QuestionCheckPoint)
                {
                    answer = await DisplayPromptAsync("Question!", response.Message);

                    if (string.IsNullOrEmpty(answer))
                    {
                        throw new RaceVenturaApiException("No answer entered.", ErrorCodes.AnswerIncorrect);
                    }

                    await _raceVenturaApiClient.RegisterPoint(parsedResult.RaceId, viewModel.Item.UniqueId, parsedResult.PointId, location.Latitude, location.Longitude, answer);
                }
                else
                {
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        await DisplayAlert("Message", response.Message, "Ok");
                    }
                }

                await DisplayAlert("Congratulations", "Point registered! Good luck finding the next point!", "Ok");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "RaceVentura is not allowed to access your location. This is needed when registering points to check if you are on the right loction. Please go to settings and grant RaceVentura access to your location.", "Ok");
            }
            catch (RaceVenturaApiException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.Duplicate:
                        await DisplayAlert("Error", "This point is already registered for your team! Quickly move on to the next point!", "Ok");
                        break;

                    case ErrorCodes.CoordinatesIncorrect:
                        await DisplayAlert("Error", "You are not at the right location for this point. Make sure you are on the right spot!", "Ok");
                        break;

                    case ErrorCodes.AnswerIncorrect:
                        await DisplayAlert("Error", $"The answer '{answer}' is incorrect!", "Ok");
                        break;

                    case ErrorCodes.NotActiveStage:
                        await DisplayAlert("Error", "You cannot register this point because it is not in the stage you are doing right now!", "Ok");
                        break;

                    case ErrorCodes.RaceNotStarted:
                        await DisplayAlert("Error", "You cannot register this point because the race did not start yet!", "Ok");
                        break;

                    default:
                        throw new Exception("Unknown error code.");
                }
            }
        }

        private async Task HandleRegisterStageEnd(QrCodeResult parsedResult)
        {
            try
            {
                var result = await DisplayAlert("Important", "You are about to finish a stage. Afterwards you cannot register anymore points for this stage and it cannot be undone. Are you sure you want to continue?", "Yes", "No");

                if (result)
                {
                    var response = await _raceVenturaApiClient.RegisterStageEnd(parsedResult.RaceId, viewModel.Item.UniqueId, parsedResult.StageId);

                    await DisplayAlert("Done", "The stage is closed, have fun with the next one!", "Ok");
                }
            }
            catch (RaceVenturaApiException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.NotActiveStage:
                        await DisplayAlert("Error", "The QR code you scanned to end the stage is not the right code for the stage you are in now!", "Ok");
                        break;

                    case ErrorCodes.RaceNotStarted:
                        await DisplayAlert("Error", "You cannot end this stage because the race has not started yet!", "Ok");
                        break;

                    default:
                        throw new Exception("Unknown error code.");
                }
            }
        }

        private async Task HandleRegisterRaceEnd(QrCodeResult parsedResult)
        {
            try
            {
                var result = await DisplayAlert("Important", "You are about to finish the race and this cannot be undone. Are you sure you want to continue?", "Yes", "No");

                if (result)
                {
                    var response = await _raceVenturaApiClient.RegisterRaceEnd(parsedResult.RaceId, viewModel.Item.UniqueId);

                    await EndRace("You finished the race! Congratulations, now go take a well deserved shower!");
                }
            }
            catch (RaceVenturaApiException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.RaceNotStarted:
                        await DisplayAlert("Error", "You cannot end this stage because the race has not started yet!", "Ok");
                        break;

                    case ErrorCodes.RaceEnded:
                        await EndRace("The race is over! Congratulations, now go take a well deserved shower!");
                        break;

                    default:
                        throw new Exception("Unknown error code.");
                }
            }
        }

        private async Task EndRace(string message)
        {
            viewModel.RaceActive = false;
            await DisplayAlert("Done", message, "Ok");
        }

        private async void ResultButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                _raceVenturaApiClient.GoToResultPage(viewModel.Item.RaceId);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Something went wrong when opening the browser.", "Ok");
            }
        }

        private async void ShareRaceResults_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                _raceVenturaApiClient.ShareRaceResults(viewModel.Item.RaceId);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Something went wrong while sharing the results.", "Ok");
            }
        }

        private async void RemoveRace_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                var result = await DisplayAlert("Important", "You are about to remove the race from your device. Are you sure you want to continue?", "Yes", "No");

                if (result)
                { 
                    MessagingCenter.Send(this, "RemoveItem", viewModel.Item);

                    await Navigation.PopToRootAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Something went wrong while deleting the race.", "Ok");
            }
        }
    }
}
