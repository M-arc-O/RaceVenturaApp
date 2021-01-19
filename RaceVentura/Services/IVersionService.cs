using System.Threading.Tasks;

namespace RaceVentura.Services
{
    public interface IVersionService
    {
        Task<bool> UpdateAvailable();
    }
}
