using Microsoft.AspNetCore.Mvc;

namespace Student.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register( )
        {
            return View();
        }
    }
}
