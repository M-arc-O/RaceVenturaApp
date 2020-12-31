using System;
using System.Threading.Tasks;
using RaceVentura.RaceVenturaApiModels;

namespace RaceVentura.Services
{
    public interface IRaceVenturaApiClient
    {
        Task<RegisterToRaceModel> RegisterToRace(Guid raceId, Guid teamId, Guid uniqueId);
    }
}
