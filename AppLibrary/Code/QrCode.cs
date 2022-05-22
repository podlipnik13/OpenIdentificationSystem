using QRCoder;

namespace AppLibrary.Code;

public class QrCode {

    public static string GetQrImage(string str){
        
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        return Convert.ToBase64String(new PngByteQRCode(qrGenerator.CreateQrCode(str, QRCodeGenerator.ECCLevel.Q)).GetGraphic(20) as byte[]);
    }
}