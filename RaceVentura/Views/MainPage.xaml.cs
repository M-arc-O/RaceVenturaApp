using System;
using System.Threading.Tasks;
using RaceVentura.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class MainPage : TabbedPage
    {
        private const string appStoreUri = "https://apps.apple.com/us/app/raceventura/id1547990194?itsct=apps_box&itscg=30200";
        private const string playStoreUri = "https://play.google.com/store/apps/details?id=com.companyname.raceventura&pcampaignid=pcampaignidMKT-Other-global-all-co-prtnr-py-PartBadge-Mar2515-1";
        private readonly IVersionService _versionService;

        public MainPage()
        {
            InitializeComponent();

            App.Current.UserAppTheme = OSAppTheme.Unspecified;
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;

            _versionService = DependencyService.Get<IVersionService>();
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            App.Current.UserAppTheme = e.RequestedTheme;
        }

        public async Task CheckVersion()
        {
            try
            {
                if (await _versionService.UpdateAvailable())
                {
                    var result = await DisplayAlert("Warning", "There is a new version of the app available in the store. Would you like to go there?", "Yes", "No");

                    if (result)
                    {
                        var uri = appStoreUri;

                        if (Device.RuntimePlatform == Device.Android)
                        {
                            uri = playStoreUri;
                        }

                        await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
