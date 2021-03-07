using System;

namespace RaceVentura.RaceVenturaApiModels
{
    public class RegisterPointModel
    {
        public Guid RaceId { get; set; }
        public Guid UniqueId { get; set; }
        public Guid PointId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PointTypeViewModel Type { get; set; }
        public string Message { get; set; }
        public string Answer { get; set; }
    }
}
