using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RaceVentura.Services
{
    public interface ILocationService
    {
        Task<Location> GetLocation();

        void CancelGetLocation();
    }
}
