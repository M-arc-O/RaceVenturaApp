using Xamarin.Forms;
using RaceVentura.Services;
using RaceVentura.Views;
using Xamarin.Essentials;

namespace RaceVentura
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<SQLiteDataStore>();
            DependencyService.Register<ParseQrCodeResultService>();
            DependencyService.Register<RaceVenturaApiClient>();
            DependencyService.Register<LocationService>();
            DependencyService.Register<VersionService>();

            VersionTracking.Track();

            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            await (MainPage as MainPage).CheckVersion();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
