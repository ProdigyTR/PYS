using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper;
using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.Models.HR;
using PerformansYonetimSistemi.ViewModels;
using System.Net.Mail;
using System.Text;

namespace PerformansYonetimSistemi.Controllers.Defination
{
    public class EvaluationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }
        MainViewModel mvm;
        #region Evaluation
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.CurrentPage = "/Evaluation/Create";
            mvm = new MainViewModel
            {
                Evaluations = _context.Evaluations.ToList()
            };
            return View(mvm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create(Evaluation evaluation)
        {
            if (_context.Evaluations.Where(w => w.Code == evaluation.Code).Any())
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = evaluation.Code + " kodu ile daha önce kayıt yapılmıştır. Başka kod kullanarak kayıt yapınız.";
                return View("Error");
            }
            _context.Add(evaluation);
            _context.SaveChanges();
            return RedirectToAction("Create");
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(string Code)
        {
            Evaluation evaluation = _context.Evaluations.FirstOrDefault(w => w.Code == Code);
            _context.Remove(evaluation);
            _context.SaveChanges();
            return RedirectToAction("Create");
        }
        #endregion
        #region Send Forms
        [Authorize]
        public async Task<IActionResult> CheckForms(string Code)
        {
            ViewBag.CurrentPage = "/Evaluation/Create";
            mvm = new MainViewModel
            {
                FormMases = await _context.FormMas.ToListAsync(),
                Evaluations = await _context.Evaluations.Where(w => w.Code == Code).ToListAsync(),
            };

            return View(mvm);
        }
        [Authorize]
        public async Task<IActionResult> ShowForm(int id, string Code)
        {
            ViewBag.CurrentPage = "/Evaluation/Create";
            mvm = new MainViewModel
            {
                FormMases = await _context.FormMas.Where(w => w.Id == id).ToListAsync(),
                FormDetails = await _context.FormDetails.Where(w => w.MasId == id).ToListAsync(),
                Evaluations = await _context.Evaluations.Where(w => w.Code == Code).ToListAsync()
            };
            return View(mvm);
        }
        [Authorize]
        public async Task<IActionResult> SendSelection(int id, string Code)
        {
            ViewBag.CurrentPage = "/Evaluation/Create";
            mvm = new MainViewModel
            {
                FormMases = await _context.FormMas.Where(w => w.Id == id).ToListAsync(),
                Employees = await _context.Employees.ToListAsync(),
                Evaluations = await _context.Evaluations.Where(w => w.Code == Code).ToListAsync()
            };
            return View(mvm);
        }
        [Authorize]
        public void createEvaluationForm(int id, string Code)
        {
            EvaluationForm evaluationForm = new EvaluationForm();
            evaluationForm.FormId = id;
            evaluationForm.EvaluationCode = Code;
            _context.Add(evaluationForm);
            _context.SaveChanges();
        }
        [Authorize]
        public void sendMail(int id, string employees, string btnAction, string Code)
        {
            createEvaluationForm(id, Code);
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
                html = "https://pys.ekmasconnect.com:797/Evaluation/NeedToFillForm?guid=";
                string guid = Guid.NewGuid().ToString();
                foreach (var item in mvm.FormDetails.Where(w => w.TextLevel == 0).OrderBy(o => o.Sequence))
                {
                    needToFillForm = new NeedToFillForm();
                    needToFillForm.Guid = guid;
                    needToFillForm.MasId = id;
                    needToFillForm.DetailId = item.Id;
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
                        needToFillForm.CreatedBy = User.Identity.Name == null ? "mert.baggbasi" : User.Identity.Name;

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
                html = "<p>Aşağıdaki linkten performans değerlendirme formunu doldurabilirsiniz</p>" + "<p><a href=\"" + html + "\">" + html + "</a></p>";
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
                NeedToFillForms = _context.NeedToFillForms.Where(w => w.Guid == guid).ToList(),


            };
            mvm.Employees = _context.Employees.Where(w => w.TC == mvm.NeedToFillForms.Select(s => s.Employee).FirstOrDefault()).ToList();
            mvm.FormMases = _context.FormMas.Where(w => w.Id == mvm.NeedToFillForms.Select(s => s.MasId).FirstOrDefault()).ToList();
            mvm.FormDetails = _context.FormDetails.Where(w => w.MasId == mvm.FormMases.Select(s => s.Id).FirstOrDefault()).ToList();
            if (mvm.NeedToFillForms.Where(w => w.ModifiedAt != null).Any())
            {
                return RedirectToAction("AlreadyCompleted", "Evaluation");
            }
            return View(mvm);
        }

        [HttpPost]
        public IActionResult UpdateNeedToFillForm(string stringData, string guid, NeedToFillDepartmentManager needToFillDepartmentManager)
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
                    if (item.Item2.GetType() == typeof(int))
                    {

                        NeedToFillForm needToFillForm = new NeedToFillForm();
                        needToFillForm = _context.NeedToFillForms.Where(w => w.Guid == guid && w.DetailId == Convert.ToInt32(item.Item1)).FirstOrDefault();
                        needToFillForm.GivenPoint = item.Item2;
                        needToFillForm.ModifiedAt = DateTime.Now;
                        _context.Update(needToFillForm);
                        _context.SaveChanges();
                    }
                }
                needToFillDepartmentManager.MasId = _context.NeedToFillForms.Where(w => w.Guid == guid).Select(s => s.MasId).FirstOrDefault();
                needToFillDepartmentManager.Guid = guid;
                needToFillDepartmentManager.CreatedAt = DateTime.Now;
                needToFillDepartmentManager.CreatedBy = "mert.bagbasi";
                _context.Add(needToFillDepartmentManager);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.errorTitle = "Hata";
                ViewBag.error = e.Message;
                return View("Error");
            }
            return RedirectToAction("FormCompleted", "Evaluation");
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
