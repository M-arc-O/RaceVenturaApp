using System;
namespace RaceVentura.Models
{
    public class QrCodeResult
    {
        public QrCodeType QrCodeType { get; set; }
        public Guid RaceId { get; set; }
        public Guid TeamId { get; set; }
        public Guid PointId { get; set; }
        public Guid StageId { get; set; }
    }
}
