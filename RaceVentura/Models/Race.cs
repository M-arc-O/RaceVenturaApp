using System;
using System.Collections.Generic;
using SQLite;

namespace RaceVentura.Models
{
    [Table("Races")]
    public class Race
    {
        [PrimaryKey]
        public Guid RaceId { get; set; }
        public string Name { get; set; }
        public Guid UniqueId { get; set; }
        public bool RaceActive { get; set; }
        public List<Point> Points { get; set; }
    }
}
