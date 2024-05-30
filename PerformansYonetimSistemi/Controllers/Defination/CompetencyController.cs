using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.Models.HR;
using PerformansYonetimSistemi.ViewModels;
using System;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class CompetencyController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CompetencyController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;

        #region Select Department&Employee
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Filter()
        {
            ViewBag.CurrentPage = "/Competency/Filter";
            mvm = new MainViewModel
            {
                KPIs = await _context.KPIs.Where(w => w.IsActive).ToListAsync(),
                Employees = await _context.Employees.Where(w => w.IsActive).OrderBy(t => t.Name).ToListAsync(),
                FormMases = await _context.FormMas.ToListAsync()
            };
            mvm.Departments = await _context.Departments.Where(w => w.IsActive && mvm.KPIs.Select(s => s.Departmant).Contains(w.Code)).OrderBy(t => t.Name).ToListAsync();
            return View(mvm);
        }
        #endregion
        #region Select Related Target Period
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Period(string DepartmanCode, string TC, int CompetencyId)
        {
            ViewBag.CurrentPage = "/Competency/Filter";
            mvm = new MainViewModel
            {
                TargetPeriods = await _context.TargetPeriods.Where(w => w.IsActive && w.Department == DepartmanCode && w.Employee == TC).ToListAsync()
            };
            ViewBag.Department = DepartmanCode;
            ViewBag.Employee = TC;
            ViewBag.CompetencyId = CompetencyId;
            return View(mvm);
        }
        #endregion
        #region Show Competency
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ShowCompetency(string TC, int CompetencyId, int TargetPeriodId)
        {
            ViewBag.TargetPeriodId = TargetPeriodId;

            if (!_context.Competencys.Where(w => w.TargetPeriodId == TargetPeriodId).Any())
            {
                List<FormDetail> form = _context.FormDetails.Where(w=>w.MasId==CompetencyId).ToList();
                
                foreach (var item in form)
                {
                    Competency competency = new Competency();
                    competency.TargetPeriodId= TargetPeriodId;
                    competency.MasId = item.MasId;
                    competency.DetailId = item.Id;
                    competency.Explanation=item.Explanation;
                    competency.TextLevel = item.TextLevel;
                    competency.Title=item.Title;
                    competency.LowerTitle = item.LowerTitle;
                    competency.Sequence = item.Sequence;
                    competency.Employee = TC;
                    competency.CreatedBy = User.Identity.Name;
                    _context.Add(competency);
                    _context.SaveChanges();
                }
            }

            mvm = new MainViewModel
            {
                Competencys = _context.Competencys.Where(w => w.TargetPeriodId == TargetPeriodId).ToList(),
            };
            
            mvm.Employees = _context.Employees.Where(w => w.TC == mvm.Competencys.Select(s => s.Employee).FirstOrDefault()).ToList();
            mvm.Departments = _context.Departments.Where(w => w.Code == mvm.Employees.Select(s => s.Department).FirstOrDefault()).ToList();
            mvm.Positions= _context.Positions.Where(w => w.Code == mvm.Employees.Select(s => s.Position).FirstOrDefault()).ToList();
            mvm.FormMases = _context.FormMas.Where(w => w.Id == mvm.Competencys.Select(s => s.MasId).FirstOrDefault()).ToList();
            mvm.FormDetails = _context.FormDetails.Where(w => w.MasId == mvm.FormMases.Select(s => s.Id).FirstOrDefault()).ToList();
            
            return View(mvm);
        }
        #endregion
        #region Create & Save
        [Authorize]
        [HttpPost]
        public IActionResult CreateAndSave(string stringData,int targetPeriodId)
        {
            int masId = 0;
            try
            {
                List<Tuple<string, string>> givenPoints = new List<Tuple<string, string>>();
                string[] pointData = stringData.Split(',');
                foreach (string data in pointData)
                {
                    string[] parts = data.Split('-');
                    if (parts.Length == 2)
                    {
                        string id = parts[0];
                        string value = parts[1];
                        givenPoints.Add(new Tuple<string, string>(id, value));
                    }
                }

                foreach (var item in givenPoints)
                {
                    if (item.Item2 != "")
                    {
                        Competency competency = new Competency();
                        competency = _context.Competencys.Where(w => w.TargetPeriodId == targetPeriodId && w.DetailId == Convert.ToInt32(item.Item1)).FirstOrDefault();
                        competency.GivenPoint = item.Item2;
                        competency.ModifiedAt = DateTime.Now;
                        _context.Update(competency);
                        _context.SaveChanges();
                    }
                }                
            }
            catch (Exception e)
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = e.Message;
                return View("Error");
            }
            string employee = _context.TargetPeriods.Where(w => w.Id == targetPeriodId).Select(s => s.Employee).FirstOrDefault();
            return RedirectToAction("PerformanceReviews", "Employee", new {TC= employee });
        }
        #endregion
    }
}
