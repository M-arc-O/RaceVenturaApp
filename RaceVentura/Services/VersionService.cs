using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RaceVentura.Services
{
    public class VersionService: IVersionService
    {
        private readonly IRaceVenturaApiClient _raceVenturaApiClient;

        public VersionService()
        {
            _raceVenturaApiClient = DependencyService.Get<IRaceVenturaApiClient>();
        }

        public async Task<bool> UpdateAvailable()
        {
            var latestVersion = await _raceVenturaApiClient.GetAppLatestVersion();
            return !latestVersion.Equals(VersionTracking.CurrentVersion);
        }
    }
}
