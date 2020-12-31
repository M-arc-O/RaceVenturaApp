using System;
using RaceVentura.Models;
using RaceVentura.Services;
using RaceVentura.ViewModels;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class RaceDetailPage : ContentPage
    {
        private readonly IParseQrCodeResultService _qrCodeResultParser;
        private readonly IRaceVenturaApiClient _raceVenturaApiClient;

        RaceDetailViewModel viewModel;

        public RaceDetailPage(RaceDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;

            _qrCodeResultParser = DependencyService.Get<IParseQrCodeResultService>();
            _raceVenturaApiClient = DependencyService.Get<IRaceVenturaApiClient>();
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
                    case Models.QrCodeType.RegisterToRace:
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
