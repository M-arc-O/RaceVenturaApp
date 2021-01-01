using System;
using System.Threading.Tasks;
using RaceVentura.Models;
using RaceVentura.RaceVenturaApiModels;
using RaceVentura.Services;
using RaceVentura.ViewModels;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class RaceDetailPage : ContentPage
    {
        private readonly IParseQrCodeResultService _qrCodeResultParser;
        private readonly IRaceVenturaApiClient _raceVenturaApiClient;
        private readonly ILocationService _locationService;

        RaceDetailViewModel viewModel;

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
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                scanner.FlashButtonText = "Enable flashlight";
                var result = await scanner.Scan();

                var parsedResult = _qrCodeResultParser.ParseResult(result.Text);

                switch (parsedResult.QrCodeType)
                {
                    case QrCodeType.RegisterPoint:
                        await HandleRegisterPoint(parsedResult);
                        break;

                    case QrCodeType.RegisterStageEnd:
                        await DisplayAlert("Yes!", "This is a stage end QR code.", "Ok");
                        break;

                    case QrCodeType.RegisterRaceEnd:
                        await DisplayAlert("Yes!", "This is a race end QR code.", "Ok");
                        break;

                    case QrCodeType.RegisterToRace:
                        await DisplayAlert("Error", "This is a race registration QR code.", "Ok");
                        break;

                    default:
                        await DisplayAlert("Error", "Unknown QR code type.", "Ok");
                        break;
                }
            }
            catch
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Something went wrong while scanning the QR code.", "Ok");
                });
            }
        }

        private async Task HandleRegisterPoint(QrCodeResult parsedResult)
        {
            var answer = string.Empty;
            try
            {
                await _locationService.GetLocation();

                var response = await _raceVenturaApiClient.RegisterPoint(parsedResult.RaceId, viewModel.Item.UniqueId, parsedResult.PointId, 0, 0, string.Empty);

                if (!string.IsNullOrEmpty(response.Question))
                {
                    answer = await DisplayPromptAsync("Question!", response.Question);

                    if (string.IsNullOrEmpty(answer))
                    {
                        throw new RaceVenturaApiException("No answer entered.", ErrorCodes.AnswerIncorrect);
                    }

                    await _raceVenturaApiClient.RegisterPoint(parsedResult.RaceId, viewModel.Item.UniqueId, parsedResult.PointId, 0, 0, answer);
                }

                await DisplayAlert("Congratulations", "Point registered! Good luck finding the next point!", "Ok");
            }
            catch (RaceVenturaApiException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.Duplicate:
                        await DisplayAlert("Error", "This point is already registered for your team! Quickly move on to the next point!", "Ok");
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

        private void ResultButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                _raceVenturaApiClient.GoToResultPage(viewModel.Item.RaceId);
            }
            catch (Exception)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Something went wrong when opening a browser.", "Ok");
                });
            }
        }
    }
}
