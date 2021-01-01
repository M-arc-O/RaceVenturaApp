using System.Threading.Tasks;

namespace RaceVentura.Services
{
    public interface ILocationService
    {
        Task GetLocation();

        void CancelGetLocation();
    }
}
