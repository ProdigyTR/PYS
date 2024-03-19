using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.ViewModels;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class KPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        public KPIController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        [HttpGet]
        public async Task<IActionResult> DepartmanFilter()
        {
            ViewBag.CurrentPage = "/KPI/DepartmanFilter";
            mvm = new MainViewModel
            {
                Departments = await _context.Departments.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync()
            };
            return View(mvm);
        }
        [HttpGet]
        public async Task<IActionResult> List(string Code)
        {
            ViewBag.CurrentPage = "/KPI/DepartmanFilter";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive && w.Departmant==Code).OrderBy(t => t.Name).ToListAsync()
            };
            ViewBag.Departmant = Code;
            return View(mvm);
        }
        [HttpPost]
        public IActionResult Create(KPI kpi)
        {
            if (_context.KPIs.Where(w => w.Code == kpi.Code).Any())
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = kpi.Code + " kodu ile daha önce kayıt yapılmıştır. Başka kod kullanarak kayıt yapınız.";
                return View("Error");
            }
            _context.Add(kpi);
            _context.SaveChanges();
            return RedirectToAction("List", new { Code =kpi.Departmant});
        }
        [HttpPost]
        public IActionResult Delete(string Code)
        {
            KPI kpi = _context.KPIs.FirstOrDefault(w => w.Code == Code);
            string departmant=kpi.Departmant;
            _context.Remove(kpi);
            _context.SaveChanges();
            return RedirectToAction("List", new { Code = departmant });
        }
        public async Task<IActionResult> Card()
        {
            ViewBag.CurrentPage = "/KPI/Card";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive).ToListAsync(),
                Targets = await _context.Targets.Where(w => w.IsActive).ToListAsync()
            };
            mvm.Employees = await _context.Employees.Where(w => w.IsActive && mvm.Targets.Select(s => s.Employee).Contains(w.TC)).ToListAsync();
            return View(mvm);
        }
        [HttpPost]
        public async Task<IActionResult> CardToEmployee(string TC, string route)
        {
            int kpiId = 0;
            if (route == "new")
            {
                EmployeeKpi employeeKpi = new EmployeeKpi();
                employeeKpi.TC = TC;
                employeeKpi.CreatedBy = "mert.bagbasi";
                _context.Add(employeeKpi);
                _context.SaveChanges();

                kpiId = _context.EmployeeKpis.Where(w => w.TC == TC).OrderByDescending(o => o.Id).Select(s => s.Id).FirstOrDefault();

                return RedirectToAction("EmployeeCard", new { TC = TC, kpiId = kpiId });
            }
            else if (route == "edit")
            {
                return RedirectToAction("EmployeeCardEdit", new { TC = TC });
            }
            return RedirectToAction("EmployeeCard");
        }
        public async Task<IActionResult> EmployeeCardEdit(string TC)
        {
            ViewBag.CurrentPage = "/KPI/Card";
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w => w.TC == TC).ToListAsync()
            };
            mvm.Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w => w.IsActive && mvm.Targets.Select(s => s.Name).Contains(w.Code)).ToListAsync();
            mvm.EmployeeKpis = await _context.EmployeeKpis.Where(w => w.TC == TC).ToListAsync();
            mvm.KpiScoreCards = await _context.KpiScoreCards.Where(w => mvm.EmployeeKpis.Select(s => s.Id).Contains(w.KpiId)).ToListAsync();
            return View(mvm);
        }
        public async Task<IActionResult> EmployeeCard(string TC, int kpiId)
        {
            ViewBag.CurrentPage = "/KPI/Card";
            mvm = new MainViewModel
            {
                Employees = await _context.Employees.Where(w => w.TC == TC).ToListAsync(),
                KpiScoreCards = await _context.KpiScoreCards.Where(w => w.KpiId == kpiId).ToListAsync()
            };
            mvm.Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w => w.IsActive && mvm.Targets.Select(s => s.Name).Contains(w.Code)).ToListAsync();
            mvm.EmployeeKpis = await _context.EmployeeKpis.Where(w => w.Id == kpiId).ToListAsync();
            return View(mvm);
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeCard(KpiScoreCard kpiScoreCard)
        {
            kpiScoreCard.CreatedBy = "mert.bagbasi";
            if (!_context.KpiScoreCards.Where(w => w.KpiId == kpiScoreCard.KpiId && w.Target == kpiScoreCard.Target).Any())
            {
                _context.Add(kpiScoreCard);
                _context.SaveChanges();
            }
            string TC = _context.EmployeeKpis.FirstOrDefault(f => f.Id == kpiScoreCard.KpiId).TC;
            return RedirectToAction("EmployeeCard", new { TC = TC, kpiId = kpiScoreCard.KpiId });
        }
        public async Task<IActionResult> ScoreCard()
        {
            ViewBag.CurrentPage = "/KPI/ScoreCard";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive).ToListAsync(),
                Targets = await _context.Targets.Where(w => w.IsActive).ToListAsync(),
                Positions = await _context.Positions.Where(w => w.IsActive).ToListAsync(),
                Departments = await _context.Departments.Where(w => w.IsActive).ToListAsync(),
                EmployeeKpis = await _context.EmployeeKpis.ToListAsync(),
                KpiScoreCards = await _context.KpiScoreCards.ToListAsync()
            };
            mvm.Employees = await _context.Employees.Where(w => w.IsActive && mvm.Targets.Select(s => s.Employee).Contains(w.TC)).ToListAsync();
            return View(mvm);
        }
        public async Task<IActionResult> DeleteKpiScoreCard(int Id)
        {
            KpiScoreCard kpiScoreCard = _context.KpiScoreCards.Where(w => w.Id == Id).FirstOrDefault();
            int kpiId = kpiScoreCard.KpiId;
            _context.Remove(kpiScoreCard);
            _context.SaveChanges();
            string TC = await _context.EmployeeKpis.Where(w => w.Id == kpiId).Select(s => s.TC).FirstOrDefaultAsync();
            return RedirectToAction("EmployeeCard", new { TC = TC, KpiId = kpiId });
        }
        [HttpPost]
        public IActionResult UpdateProgress(int Progress, int Id)
        {
            KpiScoreCard kpiScoreCard = _context.KpiScoreCards.Where(w => w.Id == Id).FirstOrDefault();
            kpiScoreCard.Progress = Progress;
            _context.Update(kpiScoreCard);
            _context.SaveChanges();
            return RedirectToAction("ScoreCard");
        }
        [HttpPost]
        public IActionResult UpdateRatio(int Ratio, int Id)
        {
            KpiScoreCard kpiScoreCard = _context.KpiScoreCards.Where(w => w.Id == Id).FirstOrDefault();
            kpiScoreCard.Ratio = Ratio;
            _context.Update(kpiScoreCard);
            _context.SaveChanges();
            return RedirectToAction("ScoreCard");
        }
    }
}