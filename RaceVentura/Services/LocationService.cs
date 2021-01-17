using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace RaceVentura.Services
{
    public class LocationService : ILocationService
    {
        CancellationTokenSource cts;

        public async Task<Location> GetLocation()
        {
            try
            {
                var status = await CheckAndRequestPermissionAsync(new Permissions.LocationWhenInUse());

                if (status != PermissionStatus.Granted)
                {
                    throw new PermissionException("Location permision not granted.");
                }

                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                return await Geolocation.GetLocationAsync(request, cts.Token);
            }
            catch (FeatureNotSupportedException)
            {
                throw;
            }
            catch (FeatureNotEnabledException)
            {
                throw;
            }
        }

        public void CancelGetLocation()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }

        private async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                if (permission.ShouldShowRationale())
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "The app needs access to your location.", "Ok");
                    });
                }

                status = await permission.RequestAsync();
            }

            return status;
        }
    }
}
