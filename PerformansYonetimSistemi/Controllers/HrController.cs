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
        public IActionResult Modules()
        {
            return View();
        }
        #endregion
        #region Form
        public IActionResult Form()
        {
            ViewBag.CurrentPage = "/Hr/Form";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.ToList()
            };

            return View(mvm);
        }
        [HttpPost]
        public IActionResult CreateForm(FormMas form)
        {
            form.CreatedAt = DateTime.Now;
            form.CreatedBy = "mert.bagbasi";
            _context.Add(form);
            _context.SaveChanges();
            return RedirectToAction("Form");
        }
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
        [HttpPost]
        public IActionResult CreateFormDetail(FormDetail formDetail)
        {
            formDetail.CreatedAt = DateTime.Now;
            formDetail.CreatedBy = "mert.bagbasi";
            _context.Add(formDetail);
            _context.SaveChanges();
            return RedirectToAction("EditForm", "Hr", new { id = formDetail.MasId });
        }
        #endregion
        #region Employee
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
        #region Evaluation
        public IActionResult CreateEvaluation()
        {
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.ToList()
            };
            return View(mvm);
        }
        #endregion
        #region Send Forms
        public IActionResult CheckForms()
        {
            ViewBag.CurrentPage = "/Hr/CheckForms";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.ToList()
            };

            return View(mvm);
        }
        public IActionResult ShowForm(int id)
        {
            ViewBag.CurrentPage = "/Hr/CheckForms";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.Where(w => w.Id == id).ToList(),
                FormDetails = _context.FormDetails.Where(w => w.MasId == id).ToList()
            };
            return View(mvm);
        }
        public IActionResult SendSelection(int id)
        {
            ViewBag.CurrentPage = "/Hr/CheckForms";
            mvm = new MainViewModel
            {
                FormMases = _context.FormMas.Where(w => w.Id == id).ToList()
            };
            return View(mvm);
        }
        public void sendMail(int id, string employees, string btnAction)
        {
            string html = "";
            List<Tuple<string, string, string>> employeeList = new List<Tuple<string, string, string>>();

            string[] employeeData = employees.Split(',');
                                                    
            foreach (string data in employeeData)
            {                                           
                string[] parts = data.Split(';');
                if (parts.Length == 3)
                {
                    string sicilNo = parts[0];
                    string employeeMail = parts[1];
                    string managerMail = parts[2];
                    employeeList.Add(new Tuple<string, string, string>(sicilNo, employeeMail, managerMail));
                }
            }
            mvm = new MainViewModel
            {
                FormDetails = _context.FormDetails.Where(w => w.MasId == id).ToList(),
            };

            NeedToFillForm needToFillForm;
            foreach (var employee in employeeList)
            {
                html = "https://localhost:7122/Hr/NeedToFillForm?guid=";
                string guid = Guid.NewGuid().ToString();
                foreach (var item in mvm.FormDetails.Where(w => w.TextLevel == 0).OrderBy(o => o.Sequence))
                {
                    needToFillForm = new NeedToFillForm();
                    needToFillForm.Guid = guid;
                    needToFillForm.MasId = id;
                    needToFillForm.DetailId=item.Id;
                    needToFillForm.Explanation = item.Explanation;
                    needToFillForm.TextLevel = item.TextLevel;
                    needToFillForm.Title = item.Title;
                    needToFillForm.LowerTitle = item.LowerTitle;
                    needToFillForm.Sequence = item.Sequence;
                    needToFillForm.GivenPoint = "0";
                    needToFillForm.Employee = employee.Item1;
                    if (btnAction == "manager")
                    {
                        needToFillForm.Manager = 1;
                    }
                    else
                    {
                        needToFillForm.Manager = 0;
                    }
                    needToFillForm.FormDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    needToFillForm.CreatedAt = DateTime.Now;
                    needToFillForm.CreatedBy = User.Identity.Name == null ? "mert.baggbasi" : User.Identity.Name;

                    _context.Add(needToFillForm);
                    _context.SaveChanges();
                    foreach (var itemLt in mvm.FormDetails.Where(w => w.TextLevel == 1 && w.Title == item.Id.ToString() && !string.IsNullOrEmpty(w.Title)).OrderBy(o => o.Sequence))
                    {
                        needToFillForm = new NeedToFillForm();
                        needToFillForm.Guid = guid;
                        needToFillForm.MasId = id;
                        needToFillForm.DetailId = itemLt.Id;
                        needToFillForm.Explanation = itemLt.Explanation;
                        needToFillForm.TextLevel = itemLt.TextLevel;
                        needToFillForm.Title = itemLt.Title;
                        needToFillForm.LowerTitle = itemLt.LowerTitle;
                        needToFillForm.Sequence = itemLt.Sequence;
                        needToFillForm.GivenPoint = "0";
                        needToFillForm.Employee = employee.Item1;
                        if (btnAction == "manager")
                        {
                            needToFillForm.Manager = 1;
                        }
                        else
                        {
                            needToFillForm.Manager = 0;
                        }
                        needToFillForm.FormDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        needToFillForm.CreatedAt = DateTime.Now;
                        needToFillForm.CreatedBy = User.Identity.Name==null ? "mert.baggbasi":User.Identity.Name;

                        _context.Add(needToFillForm);
                        _context.SaveChanges();
                        foreach (var itemDt in mvm.FormDetails.Where(w => w.TextLevel == 2 && w.LowerTitle == itemLt.Id.ToString() && !string.IsNullOrEmpty(w.LowerTitle)).OrderBy(o => o.Sequence))
                        {
                            needToFillForm = new NeedToFillForm();
                            needToFillForm.Guid = guid;
                            needToFillForm.MasId = id;
                            needToFillForm.DetailId = itemDt.Id;
                            needToFillForm.Explanation = itemDt.Explanation;
                            needToFillForm.TextLevel = itemDt.TextLevel;
                            needToFillForm.Title = itemDt.Title;
                            needToFillForm.LowerTitle = itemDt.LowerTitle;
                            needToFillForm.Sequence = itemDt.Sequence;
                            needToFillForm.GivenPoint = "0";
                            needToFillForm.Employee = employee.Item1;
                            if (btnAction == "manager")
                            {
                                needToFillForm.Manager = 1;
                            }
                            else
                            {
                                needToFillForm.Manager = 0;
                            }
                            needToFillForm.FormDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            needToFillForm.CreatedAt = DateTime.Now;
                            needToFillForm.CreatedBy = User.Identity.Name == null ? "mert.baggbasi" : User.Identity.Name;

                            _context.Add(needToFillForm);
                            _context.SaveChanges();
                        }
                    }
                }
                string adiSoyadi = _context.Employees.Where(w => w.IsActive && w.TC == employee.Item1).Select(s => s.Name + " " + s.LastName).FirstOrDefault();
                Functions functions = new Functions();
                adiSoyadi = functions.LatinToIng(adiSoyadi);
                html = html + guid;
                html = "<p>Aşağıdaki linkten performans değerlendirme formunu doldurabilirsiniz</p>"+"<p><a href=\""+html+"\">"+html+"</a></p>";
                SmtpClient client = new SmtpClient();
                client.Port = 587; // Genelde 587 ve 25 portları kullanılmaktadır.
                client.Host = "smtp.office365.com"; // Hostunuzun smtp için mail domaini.
                client.EnableSsl = true; // Güvenlik ayarları, host'a ve gönderilen server'a göre değişebilir.
                client.Timeout = 10000; // Milisaniye cinsten timeout
                client.DeliveryMethod = SmtpDeliveryMethod.Network; // Mailin yollanma methodu
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("destek@ekmasconnect.com", "31113142As11+"); // Burada hangi hesabı kullanarak mail yollayacaksanız onun ayarlarını yapmanız gerekiyor
                MailMessage mm = new MailMessage("destek@ekmasconnect.com", "mertbagbasi@hotmail.com", adiSoyadi + " - Performans Değerlendirme Formu", html); // Hangi mail adresinden nereye, konu ve içerik mail ayarlarını yapabilirsiniz
                mm.Bcc.Add(new System.Net.Mail.MailAddress("mert.bagbasi@ekmasgroup.com.tr"));
                mm.IsBodyHtml = true; // True: Html olarak Gönderme, False: Text olarak Gönderme
                mm.BodyEncoding = UTF8Encoding.UTF8; // UTF8 encoding ayarı
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure; // Hata olduğunda uyarı ver 
                client.Send(mm); // Mail yolla
            }
            

        }
        public IActionResult NeedToFillForm(string guid)
        {
            mvm = new MainViewModel
            {
                NeedToFillForms = _context.NeedToFillForms.Where(w=>w.Guid==guid).ToList(),
                

            };
            mvm.Employees = _context.Employees.Where(w => w.TC == mvm.NeedToFillForms.Select(s => s.Employee).FirstOrDefault()).ToList();
            mvm.FormMases = _context.FormMas.Where(w=>w.Id==mvm.NeedToFillForms.Select(s=>s.MasId).FirstOrDefault()).ToList();
            mvm.FormDetails = _context.FormDetails.Where(w => w.MasId == mvm.FormMases.Select(s => s.Id).FirstOrDefault()).ToList();
            if (mvm.NeedToFillForms.Where(w=>w.ModifiedAt!=null).Any())
            {
                return View("AlreadyCompleted");
            }
            return View(mvm);
        }
        [HttpPost]
        public IActionResult UpdateNeedToFillForm(string stringData, string guid, NeedToFillDepartmentManager needToFillDepartmentManager)
        {
            int masId = 0;
            try {
                List<Tuple<string, string>> givenPoints = new List<Tuple<string, string>>();
                string[] pointData = stringData.Split(',');
                foreach (string data in pointData)
                {
                    string[] parts = data.Split('-');
                    if (parts.Length == 2)
                    {
                        string id = parts[0];
                        string value = parts[1];
                        givenPoints.Add(new Tuple<string,  string>(id, value));
                    }
                }

                foreach(var item in givenPoints)
                {
                    NeedToFillForm needToFillForm = new NeedToFillForm();
                    needToFillForm = _context.NeedToFillForms.Where(w=>w.Guid==guid && w.DetailId==Convert.ToInt32(item.Item1)).FirstOrDefault();
                    needToFillForm.GivenPoint= item.Item2;
                    needToFillForm.ModifiedAt = DateTime.Now;
                    _context.Update(needToFillForm);
                    _context.SaveChanges();
                }
                needToFillDepartmentManager.MasId = _context.NeedToFillForms.Where(w => w.Guid == guid).Select(s=>s.MasId).FirstOrDefault();
                needToFillDepartmentManager.Guid = guid;
                needToFillDepartmentManager.CreatedAt = DateTime.Now;
                needToFillDepartmentManager.CreatedBy = "mert.bagbasi";
                _context.Add(needToFillDepartmentManager);
                _context.SaveChanges();
            }
            catch {
            return View("Error");
            }
            return RedirectToAction("FormCompleted");
        }
        public IActionResult FormCompleted()
        {
            return View();
        }
        public IActionResult AlreadyCompleted()
        {
            return View();
        }
        #endregion
    }
}