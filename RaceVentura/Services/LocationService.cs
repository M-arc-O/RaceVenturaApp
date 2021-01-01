using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RaceVentura.Services
{
    public class LocationService : ILocationService
    {
        CancellationTokenSource cts;

        public async Task GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }

        public void CancelGetLocation()
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
        }
    }
}
