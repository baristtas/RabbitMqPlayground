using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CreateExcelFile.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email,string Password)
        {
            var hasUser = await _userManager.FindByEmailAsync(Email);

            if(hasUser == null) 
            {
                return View();
            }
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, Password,true,false);

            if(!signInResult.Succeeded) 
            {
                return View();
            }
            else
            {
                Console.WriteLine("authenticated");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
