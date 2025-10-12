using Microsoft.AspNetCore.Mvc;

namespace LAN_File_Sharing_Application.Controllers
{
    public class FileSharingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
