using Xamarin.Forms;

namespace RaceVentura.Views
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();

            App.Current.UserAppTheme = OSAppTheme.Unspecified;

            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            App.Current.UserAppTheme = e.RequestedTheme;
        }
    }
}
