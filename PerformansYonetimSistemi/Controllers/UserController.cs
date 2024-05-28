using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Login;
using PerformansYonetimSistemi.ViewModels;
using System.Runtime.Intrinsics.X86;

namespace PerformansYonetimSistemi.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile(string companyId)
        {
            ViewBag.user = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.FirstName + " " + s.LastName).FirstOrDefaultAsync();
            ViewBag.username = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.UserName).FirstOrDefaultAsync();
            ViewBag.firstName = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.FirstName).FirstOrDefaultAsync();
            //ViewBag.department = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.department).FirstOrDefaultAsync();
            ViewBag.lastName = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.LastName).FirstOrDefaultAsync();
            ViewBag.email = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.EMail).FirstOrDefaultAsync();
            ViewBag.url = await _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.Image).FirstOrDefaultAsync();
            mvm = new MainViewModel
            {
                Users = await _context.Users.Where(w => w.Active).ToListAsync()
            };

            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Profile(string companyId, string username, string password, string confirmPassword, string email, string firstName, string lastName, string department)
        {
            TempData["errorPassword"] = "";
            ViewBag.companyId = companyId;
            ViewBag.user = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.FirstName + " " + s.LastName).FirstOrDefault();
            ViewBag.username = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.UserName).FirstOrDefault();
            ViewBag.firstName = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.FirstName).FirstOrDefault();
            //ViewBag.department = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.department).FirstOrDefault();
            ViewBag.lastName = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.LastName).FirstOrDefault();
            ViewBag.email = _context.Users.Where(w => w.UserName == User.Identity.Name).Select(s => s.EMail).FirstOrDefault();

            Users users = _context.Users.Where(w => w.UserName == username).First();

            users.UserName = username;
            if (password != null)
            {
                if (password == confirmPassword)
                {
                    users.Password = password;
                    TempData["errorPassword"] = "Şifre başarıyla değiştirilmiştir.";
                }
                else if (password != confirmPassword)
                {
                    TempData["errorPassword"] = "Girilen şifreler aynı değildir.";
                }
            }
            users.FirstName = firstName;
            users.LastName = lastName;
            //users.department = department;
            users.EMail = email;

            _context.SaveChanges();
            ViewBag.userCreated = "Kullanıcı oluşturulmuştur: " + username;
            return RedirectToAction("Profile", "User");
        }
        [Authorize]
        [HttpPost]
        public Task<IActionResult> ImageUpload(string companyId, IFormFile image, string username)
        {
            if (image != null)
            {

                string Filename = image.FileName;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Upload", Filename);

                var stream = new FileStream(path, FileMode.Create);
                image.CopyTo(stream);

                string url = "/Upload/" + Filename;
                Users users = _context.Users.Where(w => w.UserName == username).First();
                users.Image = url;
                _context.SaveChanges();
            }
            return Task.FromResult<IActionResult>(RedirectToAction("Profile", "User"));
        }
        [Authorize]
        [HttpGet]
        public IActionResult CreateUser()
        {
            mvm = new MainViewModel
            {
                Users = _context.Users.ToList()
            };
            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateUser(string userName, string password, string firstName, string lastName, string eMail, int userGroupId = 0, string NetsisUser = "", string NetsisPassword = "")
        {
            mvm = new MainViewModel
            {
                Users = _context.Users.ToList()
            };
            Users users = new Users
            {
                UserName = userName,
                Password = LoginHelper.Encrypt(password, userName),
                FirstName = firstName,
                LastName = lastName,
                EMail = eMail
            };
            _context.Add(users);
            _context.SaveChanges();
            return RedirectToAction("CreateUser");
        }
        [Authorize]
        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            Users users = _context.Users.FirstOrDefault(f => f.UserId == userId);
            if (users != null)
            {
                users.Active = false;
                _context.Update(users);
                _context.SaveChanges();
            }
            return RedirectToAction("CreateUser");
        }
    }
}
