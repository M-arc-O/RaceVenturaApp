using System;
using RaceVentura.Models;

namespace RaceVentura.Services
{
    public interface IParseQrCodeResultService
    {
        QrCodeResult ParseResult(string result);
    }
}
