using Xamarin.Forms;
using RaceVentura.Services;
using RaceVentura.Views;

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

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
