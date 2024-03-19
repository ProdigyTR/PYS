using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.ViewModels;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [HttpGet]
        public async Task<IActionResult> List()
        {
            ViewBag.CurrentPage = "/Department/List";
            mvm = new MainViewModel
            {
                Departments = await _context.Departments.Where(w=>w.IsActive).OrderBy(t=>t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Create(Department department)
        {
            if (_context.Departments.Where(w=>w.Code==department.Code).Any())
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = department.Code + " kodu ile daha önce kayıt yapılmıştır. Başka kod kullanarak kayıt yapınız.";
                return View("Error");
            }
            _context.Add(department);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult Delete(string Code)
        {
            Department department = _context.Departments.FirstOrDefault(w => w.Code==Code);
            _context.Remove(department);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
