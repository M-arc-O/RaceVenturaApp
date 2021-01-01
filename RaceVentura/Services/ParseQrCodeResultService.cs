using System;
using RaceVentura.Models;

namespace RaceVentura.Services
{
    public class ParseQrCodeResultService: IParseQrCodeResultService
    {
        public QrCodeResult ParseResult(string result)
        {
            var retVal = new QrCodeResult();

            var splitString = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in splitString)
            {
                var itemSplit = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                switch (itemSplit[0].Trim())
                {
                    case "QrCodeType":
                        if (!Enum.TryParse<QrCodeType>(itemSplit[1].Trim(), out var qrCodeType))
                        {
                            throw new Exception($"Could not parse {itemSplit[1].Trim()} to QrCodeType");
                        }

                        retVal.QrCodeType = qrCodeType;
                        break;

                    case "RaceId":
                        retVal.RaceId = Guid.Parse(itemSplit[1].Trim());
                        break;

                    case "StageId":
                        retVal.StageId = Guid.Parse(itemSplit[1].Trim());
                        break;

                    case "TeamId":
                        retVal.TeamId = Guid.Parse(itemSplit[1].Trim());
                        break;

                    case "PointId":
                        retVal.PointId = Guid.Parse(itemSplit[1].Trim());
                        break;

                    default:
                        throw new Exception("Unknown field in QR code result.");
                }
            }

            return retVal;
        }
    }
}
