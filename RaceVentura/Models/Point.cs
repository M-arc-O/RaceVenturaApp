using System;
using SQLite;

namespace RaceVentura.Models
{
    public class Point
    {
        [PrimaryKey]
        public Guid PointId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Registered { get; set; }
    }
}
