using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.ViewModels;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [HttpGet]
        public async Task<IActionResult> List()
        {
            ViewBag.CurrentPage = "/Employee/List";
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.OrderBy(o => o.Name).ThenBy(t=>t.LastName).ToListAsync(),
                Positions = await _context.Positions.Where(w => w.IsActive).OrderBy(o => o.DepartmentCode).ThenBy(t => t.Name).ToListAsync(),
                Departments = await _context.Departments.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpGet]
        public async Task<IActionResult> GoTo(string TC, string act)
        {
            if (act == "edit")
            {
                return RedirectToAction("Edit", new {TC=TC });
            }
            else if (act == "passive")
            {
                Employee employee = await _context.Employees.FirstOrDefaultAsync(o => o.TC==TC);
                if (employee!=null)
                {
                    employee.IsActive = false;
                    employee.ModifiedAt=DateTime.Now;
                    employee.ModifiedBy = User.Identity.Name;
                    _context.Update(employee);
                    _context.SaveChanges();
                }
                return RedirectToAction("List");
            }

            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w => w.IsActive).OrderBy(w => w.Name).ThenBy(t=>t.LastName).ToListAsync(),
                Positions= await _context.Positions.Where(w=>w.IsActive).OrderBy(o=>o.Name).ToListAsync(),
                Departments = await _context.Departments.Where(w=>w.IsActive).OrderBy(o=>o.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            if (_context.Employees.Where(w => w.TC == employee.TC).Any())
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = employee.TC + " TC numarası ile daha önce kayıt yapılmıştır. Başka TC kullanarak kayıt yapınız.";
                return View("Error");
            }
            _context.Add(employee);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string TC)
        {
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w=>w.TC==TC).ToListAsync(),
                Positions= await _context.Positions.Where(w=>w.IsActive).ToListAsync(),
                Departments= await _context.Departments.Where(w=>w.IsActive).ToListAsync()
            };
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Delete(string TC)
        {
            Employee employee = _context.Employees.FirstOrDefault(w => w.TC == TC);
            _context.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("List");
        }
    }
}