using System;

namespace RaceVentura.RaceVenturaApiModels
{
    public class RegisterStageEndModel
    {
        public Guid RaceId { get; set; }
        public Guid UniqueId { get; set; }
        public Guid StageId { get; set; }
    }
}
