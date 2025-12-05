using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> List(string Code)
        {
            ViewBag.CurrentPage = "/KPI/DepartmanFilter";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive && w.Departmant == Code).OrderBy(t => t.Name).ToListAsync()
            };
            ViewBag.Departmant = Code;
            return View(mvm);
        }
        [Authorize]
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
            return RedirectToAction("List", new { Code = kpi.Departmant });
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(string Code)
        {
            KPI kpi = _context.KPIs.FirstOrDefault(w => w.Code == Code);
            string departmant = kpi.Departmant;
            _context.Remove(kpi);
            _context.SaveChanges();
            return RedirectToAction("List", new { Code = departmant });
        }
        [Authorize]
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CardToEmployee(string TC, string route, DateTime startDate, DateTime endDate)
        {
            int kpiId = 0;
            if (route == "new")
            {
                if (_context.EmployeeKpis.Where(w => w.TC == TC && w.StartDate <= startDate && w.EndDate >= startDate).Any())
                {
                    ViewBag.errorTitle = "Hata";
                    ViewBag.error = "Başlangıç tarihi, mevcut Performans Kartı dönem aralığında olamaz..!";
                    return View("Error");
                }
                else
                {
                    EmployeeKpi employeeKpi = new EmployeeKpi();
                    employeeKpi.TC = TC;
                    employeeKpi.StartDate = startDate;
                    employeeKpi.EndDate = endDate;
                    employeeKpi.CreatedBy = "mert.bagbasi";
                    _context.Add(employeeKpi);
                    _context.SaveChanges();

                    kpiId = _context.EmployeeKpis.Where(w => w.TC == TC).OrderByDescending(o => o.Id).Select(s => s.Id).FirstOrDefault();

                    return RedirectToAction("EmployeeCard", new { TC = TC, kpiId = kpiId });
                }
            }
            else if (route == "edit")
            {
                return RedirectToAction("EmployeeCardEdit", new { TC = TC });
            }
            return RedirectToAction("EmployeeCard");
        }
        [Authorize]
        public async Task<IActionResult> EmployeeCardEdit(string TC)
        {
            ViewBag.CurrentPage = "/KPI/Card";
            mvm = new MainViewModel
            {
                Positions = await _context.Positions.Where(w => w.IsActive).ToListAsync(),
                Departments = await _context.Departments.Where(w => w.IsActive).ToListAsync(),
                Employees = await _context.Employees.Where(w => w.TC == TC).ToListAsync()
            };
            mvm.Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w => w.IsActive && mvm.Targets.Select(s => s.KpiCode).Contains(w.Code)).ToListAsync();
            mvm.EmployeeKpis = await _context.EmployeeKpis.Where(w => w.TC == TC && (w.StartDate >= DateTime.Now || w.EndDate>=DateTime.Now)).ToListAsync();
            mvm.PerformanceCards = await _context.PerformanceCards.Where(w => mvm.TargetPeriods.Select(s => s.Id).Contains(w.TargetPeriodId)).ToListAsync();
            if (!mvm.EmployeeKpis.Where(w=> mvm.PerformanceCards.Select(s=>s.TargetPeriodId).Contains(w.Id)).Any())
            {
                List<EmployeeKpi> employeeKpis = _context.EmployeeKpis.Where(w => w.TC == TC && !mvm.PerformanceCards.Select(s => s.TargetPeriodId).Contains(w.Id)).ToList();
                _context.RemoveRange(employeeKpis);
                _context.SaveChanges();
            }
            return View(mvm);
        }
        [Authorize]
        public async Task<IActionResult> EmployeeCard(string TC, int kpiId)
        {
            ViewBag.CurrentPage = "/KPI/Card";
            mvm = new MainViewModel
            {
                Positions = await _context.Positions.Where(w => w.IsActive).ToListAsync(),
                Departments = await _context.Departments.Where(w => w.IsActive).ToListAsync(),
                Employees = await _context.Employees.Where(w => w.TC == TC).ToListAsync(),
                PerformanceCards = await _context.PerformanceCards.Where(w => w.TargetPeriodId == kpiId).ToListAsync()
            };
            mvm.Targets = await _context.Targets.Where(w => w.IsActive && w.Employee == TC).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w => w.IsActive && mvm.Targets.Select(s => s.KpiCode).Contains(w.Code)).ToListAsync();
            mvm.EmployeeKpis = await _context.EmployeeKpis.Where(w => w.Id == kpiId).ToListAsync();
            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EmployeeCard(PerformanceCard PerformanceCard)
        {
            PerformanceCard.CreatedBy = "mert.bagbasi";
            if (!_context.PerformanceCards.Where(w => w.TargetPeriodId == PerformanceCard.TargetPeriodId && w.TargetId==PerformanceCard.TargetId).Any())
            {
                _context.Add(PerformanceCard);
                _context.SaveChanges();
            }
            string TC = _context.EmployeeKpis.FirstOrDefault(f => f.Id == PerformanceCard.TargetPeriodId).TC;
            return RedirectToAction("EmployeeCard", new { TC = TC, kpiId = PerformanceCard.TargetPeriodId });
        }
        [Authorize]
        public async Task<IActionResult> DeletePerformanceCard(int Id)
        {
            PerformanceCard PerformanceCard = _context.PerformanceCards.Where(w => w.Id == Id).FirstOrDefault();
            int kpiId = PerformanceCard.TargetPeriodId;
            _context.Remove(PerformanceCard);
            _context.SaveChanges();
            string TC = await _context.EmployeeKpis.Where(w => w.Id == kpiId).Select(s => s.TC).FirstOrDefaultAsync();
            return RedirectToAction("EmployeeCard", new { TC = TC, KpiId = kpiId });
        }

        #region Performance Card Filter
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PerformanceCardFilter()
        {
            ViewBag.CurrentPage = "/KPI/PerformanceCardFilter";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive).ToListAsync(),
                Employees = await _context.Employees.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync(),
            };
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && mvm.KPIs.Select(s => s.Departmant).Contains(w.Code)).OrderBy(t => t.Name).ToListAsync();
            return View(mvm);
        }
        #endregion
        #region Select Period
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Period(string DepartmanCode, string TC)
        {
            ViewBag.CurrentPage = "/KPI/PerformanceCardFilter";
            mvm = new MainViewModel
            {
                TargetPeriods = await _context.TargetPeriods.Where(w => w.IsActive && w.Department == DepartmanCode && w.Employee == TC).ToListAsync()
            };
            mvm.Targets= await _context.Targets.Where(w=>mvm.TargetPeriods.Select(s=>s.Id).Contains(w.TargetPeriodId)).ToListAsync();
            ViewBag.Department = DepartmanCode;
            ViewBag.Employee = TC;
            return View(mvm);
        }
        #endregion
        #region Adjust Calculation of the KPI Score Card
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ScoreCard(int Id)
        {
            ViewBag.CurrentPage = "/KPI/PerformanceCardFilter";
            mvm = new MainViewModel
            {
                TargetPeriods =await _context.TargetPeriods.Where(s => s.Id == Id).ToListAsync(),
               
            };
            mvm.Targets = await _context.Targets.Where(w => w.IsActive && mvm.TargetPeriods.Select(s => s.Id).Contains(w.TargetPeriodId)).ToListAsync();
            mvm.KPIs = await _context.KPIs.Where(w => w.IsActive && mvm.Targets.Select(s => s.KpiCode).Contains(w.Code)).ToListAsync();
            mvm.Employees = await _context.Employees.Where(w => w.IsActive && mvm.Targets.Select(s => s.Employee).Contains(w.TC)).ToListAsync();
            mvm.Positions = await _context.Positions.Where(w => w.IsActive && mvm.Employees.Select(s => s.Position).Contains(w.Code)).ToListAsync();
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && mvm.Employees.Select(s => s.Department).Contains(w.Code)).ToListAsync();
            mvm.PerformanceCards = await _context.PerformanceCards.Where(w=>mvm.TargetPeriods.Select(s=>s.Id).Contains(w.TargetPeriodId)).ToListAsync();            
            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpdatePoint(string pointData, int Id)
        {
            string[] pointList = pointData.Split("|");
            List<Dictionary<string, string>> resultList = new List<Dictionary<string, string>>();

            foreach (string data in pointList)
            {
                // "-" karakterlerini ayırmak için bir liste oluştur
                string[] parcalar = data.Split("-");
                string targetPeriodId = parcalar[0];
                string targetId = parcalar[1];
                string point = parcalar[2];

                Dictionary<string, string> degerler = new Dictionary<string, string>();
                degerler.Add("TargetPeriodId", targetPeriodId);
                degerler.Add("TargetId", targetId);
                degerler.Add("Point", point);

                resultList.Add(degerler);
            }
            PerformanceCard performanceCard;
            int tpId= 0;
            foreach (Dictionary<string, string> value in resultList)
            {
                performanceCard = _context.PerformanceCards.Where(w =>w.TargetPeriodId== Convert.ToInt32(value["TargetPeriodId"]) && w.TargetId == Convert.ToInt32(value["TargetId"])).FirstOrDefault();
                performanceCard.Point = Convert.ToDecimal(value["Point"].Replace(".",","));
                performanceCard.ModifiedAt = DateTime.Now;
                performanceCard.ModifiedBy = "mert.bagbasi";
                _context.Update(performanceCard);
                _context.SaveChanges();
                tpId = Convert.ToInt32(value["TargetPeriodId"]);
            }

            return RedirectToAction("ScoreCard", new {Id= tpId});
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpdateRatio(int Ratio, int Id)
        {
            PerformanceCard PerformanceCard = _context.PerformanceCards.Where(w => w.Id == Id).FirstOrDefault();
            PerformanceCard.Ratio = Ratio;
            _context.Update(PerformanceCard);
            _context.SaveChanges();
            return RedirectToAction("ScoreCard", new { Id = PerformanceCard.TargetPeriodId});
        }
        #endregion
    }
}