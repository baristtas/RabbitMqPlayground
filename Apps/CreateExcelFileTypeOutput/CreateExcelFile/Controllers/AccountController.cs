using Microsoft.AspNetCore.Mvc;

namespace CreateExcelFile.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
