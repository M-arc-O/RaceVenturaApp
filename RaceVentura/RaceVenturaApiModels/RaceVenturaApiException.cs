using System;

namespace RaceVentura.RaceVenturaApiModels
{
    public class RaceVenturaApiException : Exception
    {
        public ErrorCodes ErrorCode { get; set; }

        public RaceVenturaApiException(string message, ErrorCodes errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}