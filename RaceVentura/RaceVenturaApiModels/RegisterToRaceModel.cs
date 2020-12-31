using System;

namespace RaceVentura.RaceVenturaApiModels
{
    public class RegisterToRaceModel
    {
        public Guid RaceId { get; set; }

        public Guid TeamId { get; set; }

        public Guid UniqueId { get; set; }

        public string Name { get; set; }
    }
}
