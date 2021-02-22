using Microsoft.AspNetCore.Mvc;

namespace Octoller.BotBox.Web.Controllers 
{
    public class HomeController : Controller 
    {
        public IActionResult Index() 
        {
            return View();
        }
    }
}
