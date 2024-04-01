using Microsoft.AspNetCore.Mvc;

namespace WMBA_4.Controllers
{
    public class HelpTopic : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
