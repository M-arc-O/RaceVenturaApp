using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

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
            where T : Permissions.BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                if(permission.ShouldShowRationale())
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Warning", "The application needs access to your location. Please click allow when asked or give access to the location in the settings of your device.", "Ok");
                    });
                }

                status = await Device.InvokeOnMainThreadAsync<PermissionStatus>(() =>
                {
                    return permission.RequestAsync();
                });
            }

            return status;
        }
    }
}
