using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerformansYonetimSistemi.Helper;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.HR;
using PerformansYonetimSistemi.ViewModels;
using System.Net.Mail;
using System.Text;

namespace PerformansYonetimSistemi.Controllers
{
    public class HrController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HrController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        #region Modules
        [Authorize]
        public IActionResult Modules()
        {
            return View();
        }
        #endregion
        #region Form
        [Authorize]
        public IActionResult Form()
        {
            ViewBag.CurrentPage = "/Hr/Form";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.ToList()
            };

            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateForm(FormMas form)
        {
            form.CreatedAt = DateTime.Now;
            form.CreatedBy = "mert.bagbasi";
            _context.Add(form);
            _context.SaveChanges();
            return RedirectToAction("Form");
        }
        [Authorize]
        public IActionResult EditForm(int id)
        {
            ViewBag.CurrentPage = "/Hr/Form";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.Where(w => w.Id == id).ToList(),
                FormDetails = _context.FormDetails.Where(w => w.MasId == id).ToList()
            };
            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreateFormDetail(FormDetail formDetail)
        {
            formDetail.CreatedAt = DateTime.Now;
            formDetail.CreatedBy = "mert.bagbasi";
            _context.Add(formDetail);
            _context.SaveChanges();
            return RedirectToAction("EditForm", "Hr", new { id = formDetail.MasId });
        }
        [Authorize]
        [HttpPost]
        public IActionResult UpdateRatio(int Ratio, int Id)
        {
            FormDetail formDetail = _context.FormDetails.Where(w => w.Id == Id).FirstOrDefault();
            formDetail.Weight = Ratio;
            _context.Update(formDetail);
            _context.SaveChanges();
            return RedirectToAction("EditForm", new {id=formDetail.MasId});
        }
        #endregion
        #region Employee
        [Authorize]
        public IActionResult EmployeeList()
        {
            ViewBag.CurrentPage = "/Hr/EmployeeList";
            mvm = new MainViewModel
            {
                Employees = _context.Employees.Where(w=>w.IsActive).ToList()
            };
            return View(mvm);
        }
        #endregion
        
    }
}