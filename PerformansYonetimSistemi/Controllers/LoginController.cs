using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PerformansYonetimSistemi.Helper;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Login;
using System.Security.Claims;

namespace PerformansYonetimSistemi.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
            //return RedirectToAction("Login","Login");
        }
        [HttpPost]
        public async Task<IActionResult> Login(Users u)
        {
            var bilgiler = _context.Users.FirstOrDefault(x => x.UserName == u.UserName && (x.Password == LoginHelper.Encrypt(u.Password, u.UserName) || x.Password == u.Password));
            if (bilgiler != null)
            {
                ViewBag.errorLogin = "";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,u.UserName)
                };
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.errorLogin = "Hatalı Kullanıcı Adı/Şifre";
            return View("Index", "Login");
        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}
