using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;
public class HomeController : Controller {
    
    public IActionResult Index() {
        
        string url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api";
        string name = "posta";
        
        ViewData["Base64Qr"] = (string)QrCode.GetQrImage(url+ "|" + name);
        
        return View();
    }

}
