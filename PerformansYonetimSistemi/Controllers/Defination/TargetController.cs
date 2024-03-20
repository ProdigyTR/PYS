using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.ViewModels;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class TargetController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TargetController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [HttpGet]
        public async Task<IActionResult> DepartmanFilter()
        {
            ViewBag.CurrentPage = "/Target/DepartmanFilter";
            mvm = new MainViewModel
            {
                Departments = await _context.Departments.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpGet]
        public async Task<IActionResult> EmployeeFilter(string Code)
        {
            ViewBag.CurrentPage = "/Target/DepartmanFilter";
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w => w.IsActive && w.Department==Code).OrderBy(t => t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpGet]
        public async Task<IActionResult> List(string TC)
        {
            ViewBag.CurrentPage = "/Target/List";
            string departmant = await _context.Employees.Where(w => w.IsActive && w.TC == TC).Select(s => s.Department).FirstOrDefaultAsync();
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w=>w.IsActive && w.TC==TC).OrderBy(o=>o.Name).ThenBy(t=>t.LastName).ToListAsync(),
                Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).OrderBy(t => t.Explanation).ToListAsync(),
                KPIs = await _context.KPIs.Where(w => w.IsActive && w.Departmant == departmant).OrderBy(t => t.Name).ToListAsync()
            };
            mvm.Positions=await _context.Positions.Where(w=>w.IsActive && w.Code==mvm.Employees.Select(s=>s.Position).FirstOrDefault()).ToListAsync();
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && w.Code == mvm.Employees.Select(s => s.Department).FirstOrDefault()).ToListAsync();
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Create(Target target)
        {
            target.CreatedBy = "mert.bagbasi";
            _context.Add(target);
            _context.SaveChanges();
            return RedirectToAction("List", new {TC=target.Employee});
        }
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            Target target = _context.Targets.FirstOrDefault(w => w.Id == Id);
            string TC = target.Employee;
            _context.Remove(target);
            _context.SaveChanges();
            return RedirectToAction("List", new { TC = TC });
        }
    }
}
