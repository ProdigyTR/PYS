using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.ViewModels;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class PositionController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PositionController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [HttpGet]
        public async Task<IActionResult> List()
        {
            ViewBag.CurrentPage = "/Position/List";
            mvm = new MainViewModel
            {
                Positions = await _context.Positions.Where(w => w.IsActive).OrderBy(o => o.DepartmentCode).ThenBy(t=>t.Name).ToListAsync(),
                Departments = await _context.Departments.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Create(Position position)
        {
            if (_context.Positions.Where(w => w.Code == position.Code).Any())
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = position.Code + " kodu ile daha önce kayıt yapılmıştır. Başka kod kullanarak kayıt yapınız.";
                return View("Error");
            }
            _context.Add(position);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        [HttpPost]
        public IActionResult Delete(string Code)
        {
            Position position = _context.Positions.FirstOrDefault(w => w.Code == Code);
            _context.Remove(position);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}
