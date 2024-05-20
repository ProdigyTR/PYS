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
        #region Select Department&Employee
        [HttpGet]
        public async Task<IActionResult> Filter()
        {
            ViewBag.CurrentPage = "/Target/Filter";
            mvm = new MainViewModel
            {                
                KPIs = await _context.KPIs.Where(w=>w.IsActive).ToListAsync(),
                Employees = await _context.Employees.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync(),
            };
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && mvm.KPIs.Select(s => s.Departmant).Contains(w.Code)).OrderBy(t => t.Name).ToListAsync();
            return View(mvm);
        }
        #endregion
        #region Create || Select Period
        [HttpGet]
        public async Task<IActionResult> Period(string DepartmanCode,string TC)
        {
            ViewBag.CurrentPage = "/Target/Filter";
            mvm = new MainViewModel
            {
                TargetPeriods = await _context.TargetPeriods.Where(w=>w.IsActive && w.Department==DepartmanCode && w.Employee==TC).ToListAsync()
            };
            ViewBag.Department = DepartmanCode;
            ViewBag.Employee = TC;
            return View(mvm);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePeriod(TargetPeriod targetPeriod)
        {
            try
            {
                if (await _context.TargetPeriods.Where(w=>w.Department==targetPeriod.Department && w.Employee==targetPeriod.Employee && ((w.StartDate<=targetPeriod.StartDate && w.EndDate>targetPeriod.StartDate) || (w.StartDate<=targetPeriod.EndDate && w.EndDate>=targetPeriod.EndDate))).AnyAsync())
                {
                    ViewBag.errorTitle = "Uyarı";
                    ViewBag.error = "Seçilen tarih aralığında mevcut değerlendirme vardır. Listeden ilerleyiniz.";
                    return View("Error");
                }
                else
                {
                    _context.Add(targetPeriod);
                    _context.SaveChanges();
                    return RedirectToAction("Period", new { DepartmanCode = targetPeriod.Department, TC = targetPeriod.Employee });
                }
            }catch (Exception ex)
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = ex.Message;
                return View("Error");
            }
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> EmployeeTarget(int Id)
        {
            ViewBag.CurrentPage = "/Target/Filter";
            
            mvm = new MainViewModel
            {            
                TargetPeriods = await _context.TargetPeriods.Where(w=>w.Id==Id).ToListAsync()
            };
            string TC = mvm.TargetPeriods.Select(s => s.Employee).FirstOrDefault();
            mvm.Employees = await _context.Employees.Where(w=>w.TC==TC).ToListAsync();
            
            string position = mvm.Employees.Select(s => s.Position).FirstOrDefault();
            string department = mvm.Employees.Select(s => s.Department).FirstOrDefault();
            
            mvm.Positions=await _context.Positions.Where(w=>w.IsActive && w.Code== position).ToListAsync();
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && w.Code == mvm.Employees.Select(s => s.Department).FirstOrDefault()).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w=>w.Departmant==department).ToListAsync();
            mvm.Targets = await _context.Targets.Where(w => w.Employee == TC && w.TargetPeriodId==Id).ToListAsync();
            return View(mvm);
        }
        //[HttpGet]
        //public async Task<IActionResult> PositionTarget(string code)
        //{
        //    ViewBag.CurrentPage = "/Target/PositionTarget";
        //    mvm = new MainViewModel
        //    {
        //        Positions = await _context.Positions.Where(w=>w.IsActive).ToListAsync(),
        //        Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).OrderBy(t => t.Explanation).ToListAsync(),
        //        KPIs = await _context.KPIs.Where(w => w.IsActive && w.Departmant == departmant).OrderBy(t => t.Name).ToListAsync()
        //    };
        //    mvm.Positions = await _context.Positions.Where(w => w.IsActive && w.Code == mvm.Employees.Select(s => s.Position).FirstOrDefault()).ToListAsync();
        //    mvm.Departments = await _context.Departments.Where(w => w.IsActive && w.Code == mvm.Employees.Select(s => s.Department).FirstOrDefault()).ToListAsync();
        //    return View(mvm);
        //}
        [HttpPost]
        public IActionResult Create(Target target)
        {
            target.CreatedBy = "mert.bagbasi";
            _context.Add(target);
            _context.SaveChanges();
            return RedirectToAction("EmployeeTarget", new {TC=target.Employee});
        }
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            Target target = _context.Targets.FirstOrDefault(w => w.Id == Id);
            string TC = target.Employee;
            _context.Remove(target);
            _context.SaveChanges();
            return RedirectToAction("EmployeeTarget", new { TC = TC });
        }
    }
}
