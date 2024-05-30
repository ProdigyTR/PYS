using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EmployeeTarget(int Id)
        {
            ViewBag.CurrentPage = "/Target/Filter";

            var targetPeriods = await _context.TargetPeriods.Where(w => w.Id == Id).ToListAsync();

            string TC = targetPeriods.Select(s => s.Employee).FirstOrDefault();
            var employees = await _context.Employees.Where(w => w.TC == TC).ToListAsync();

            string position = employees.Select(s => s.Position).FirstOrDefault();
            string department = employees.Select(s => s.Department).FirstOrDefault();

            var positions = await _context.Positions.Where(w => w.IsActive && w.Code == position).ToListAsync();
            var departments = await _context.Departments.Where(w => w.IsActive && w.Code == department).ToListAsync();
            var kpis = await _context.KPIs.Where(w => w.Departmant == department).ToListAsync();
            var targets = await _context.Targets.Where(w => w.Employee == TC && w.TargetPeriodId == Id).ToListAsync();
            var departmentEmployees = await _context.Employees.Where(w => w.Department == department).OrderBy(o => o.Name).ThenBy(t=>t.LastName).ToListAsync();

            mvm = new MainViewModel
            {
                TargetPeriods = targetPeriods,
                Employees = employees,
                Positions = positions,
                Departments = departments,
                KPIs = kpis,
                Targets = targets
            };

            ViewBag.employees = departmentEmployees;

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
        [Authorize]
        [HttpPost]
        public IActionResult Create(Target target)
        {
            target.CreatedBy = "mert.bagbasi";
            _context.Add(target);
            _context.SaveChanges();

            int lastRecord = _context.Targets.OrderByDescending(o => o.Id).Select(s => s.Id).FirstOrDefault();
            PerformanceCard performanceCard = new PerformanceCard();
            performanceCard.TargetPeriodId=target.TargetPeriodId;
            performanceCard.TargetId = lastRecord;
            _context.Add(performanceCard);
            _context.SaveChanges();
            return RedirectToAction("EmployeeTarget", new { Id = target.TargetPeriodId});
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int Id)
        {
            Target target = _context.Targets.FirstOrDefault(w => w.Id == Id);
            int targetPeriodId = target.TargetPeriodId;
            string TC = target.Employee;
            _context.Remove(target);
            _context.SaveChanges();
            return RedirectToAction("EmployeeTarget", new { Id = targetPeriodId });
        }
        [Authorize]
        [HttpPost]
        public IActionResult CopyTarget(string employee,int targetPeriodId)
        {
            var targetSelected = _context.Targets.Where(f => f.TargetPeriodId == targetPeriodId).ToList();

            var targetPeriodSelected = _context.TargetPeriods.FirstOrDefault(f => f.Id == targetPeriodId);

            TargetPeriod targetPeriod = new TargetPeriod();
            
            targetPeriod.Employee = employee;
            targetPeriod.Department=targetPeriodSelected.Department;
            targetPeriod.StartDate= targetPeriodSelected.StartDate; 
            targetPeriod.EndDate= targetPeriodSelected.EndDate;
            targetPeriod.CreatedAt=DateTime.Now;
            targetPeriod.CreatedBy = User.Identity.Name;
            

            _context.Add(targetPeriod);
            _context.SaveChanges();

            int tarperId = _context.TargetPeriods.Max(m => m.Id);
            
            foreach (var item in targetSelected)
            {
                Target target = new Target();
                target.TargetPeriodId = tarperId;    
                target.Employee = employee;
                target.KpiCode= item.KpiCode;
                target.Explanation= item.Explanation;
                target.IsActive= item.IsActive;
                target.CreatedAt=DateTime.Now;
                target.CreatedBy = User.Identity.Name;

                _context.Add(target);
                _context.SaveChanges();
            }
            
            return RedirectToAction("EmployeeTarget", "Target", new { Id = tarperId });
        }
    }
}
