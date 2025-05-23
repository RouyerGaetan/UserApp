using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Text;

public class QRCodeController : Controller
{
    public IActionResult Generate(int reservationId)
    {
        var qrContent = $"https://guest-sport.com/billet/{reservationId}";

        using (var qrGenerator = new QRCodeGenerator())
        {
            var qrData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrData);
            var svg = qrCode.GetGraphic(4);

            return Content(svg, "image/svg+xml", Encoding.UTF8);
        }
    }
}
