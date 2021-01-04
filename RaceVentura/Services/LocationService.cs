using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RaceVentura.Services
{
    public class LocationService : ILocationService
    {
        CancellationTokenSource cts;

        public async Task<Location> GetLocation()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

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
    }
}
