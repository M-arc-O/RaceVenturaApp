using System;
namespace RaceVentura.Models
{
    public class Race
    {
        public string Name { get; set; }

        public Guid RaceId { get; set; }

        public Guid UniqueId { get; set; }
    }
}
