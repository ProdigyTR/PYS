using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.Models.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace PerformansYonetimSistemi.Helper
{
    public class Functions
    {
        public string LatinToIng(string text)
        {
            // Türkçe karakterleri düzelten regex ifadeleri
            string[] karakterler = { "ý", "Ý", "Þ", "þ", "Ð", "ð" };
            string[] orjinaller = { "ı", "İ", "Ş", "ş", "Ğ", "ğ" };

            for (int i = 0; i < karakterler.Length; i++)
            {
                text = text.Replace(karakterler[i], orjinaller[i]);
            }
            return text;
        }
        public string ResponsibleList(string tcNumbers, List<Employee> employeeModel)
        {
            string[] tcArray = tcNumbers.Split(',');
            List<string> result = new List<string>();

            foreach (string tc in tcArray)
            {
                Employee employee = employeeModel.FirstOrDefault(e => e.TC == tc);
                if (employee != null)
                {
                    result.Add($"{employee.Name} {employee.LastName}");
                }
            }

            return string.Join(",", result);
        }
        public double CalculateTotalPoint(List<FormDetail> formDetail, List<NeedToFillForm> needToFillForm)
        {
            double result = 0;
            double dblTxtLvl0 = 0;
            foreach (var txtLvl0 in needToFillForm.Where(w => w.TextLevel == 0).OrderBy(o => o.DetailId))
            {
                double dblTxtLvl1 = 0;
                foreach (var txtLvl1 in needToFillForm.Where(w => w.TextLevel == 1 && w.Title == txtLvl0.DetailId.ToString()).OrderBy(o => o.DetailId))
                {
                    double dblTxtLvl2 = 0;
                    foreach (var txtLvl2 in needToFillForm.Where(w => w.TextLevel == 2 && w.LowerTitle == txtLvl1.DetailId.ToString()).OrderBy(o => o.DetailId))
                    {
                        dblTxtLvl2 = dblTxtLvl2 + Convert.ToInt32(txtLvl2.GivenPoint) * formDetail.Where(w => w.Id == txtLvl2.DetailId).Select(s => s.Weight).FirstOrDefault() / 100.00;
                    }
                    dblTxtLvl1 = dblTxtLvl1 + dblTxtLvl2 * formDetail.Where(w => w.Id == txtLvl1.DetailId).Select(s => s.Weight).FirstOrDefault() / 100.00;
                }
                dblTxtLvl0 = dblTxtLvl0 + dblTxtLvl1 * formDetail.Where(w => w.Id == txtLvl0.DetailId).Select(s => s.Weight).FirstOrDefault();
            }
            result = dblTxtLvl0;
            return result;
        }
        public double CalculatePoint(int Id, List<FormDetail> formDetail, List<NeedToFillForm> needToFillForm)
        {
            double result = 0;

            int textLvl = formDetail.Where(w => w.Id == Id).Select(s => s.TextLevel).FirstOrDefault();

            double decTextLvl0 = 0;
            double decTextLvl1 = 0;

            if (textLvl == 1)
            {
                foreach (var item in needToFillForm.Where(w => formDetail.Where(w => Convert.ToInt32(w.LowerTitle) == Id).Select(s => s.Id).Contains(w.DetailId)))
                {
                    decTextLvl1 += Convert.ToDouble(item.GivenPoint) * formDetail.Where(w => w.Id == item.DetailId).Select(s => Convert.ToDouble(s.Weight)).FirstOrDefault() / 100.0;
                }
                result = Math.Round(decTextLvl1 * formDetail.Where(w => w.Id == Id).Select(s => Convert.ToDouble(s.Weight)).FirstOrDefault() / 100.0, 2);
                result = Math.Round(result / 5 * 100.0, 2);
            }
            else if (textLvl == 0)
            {
                foreach (var item in needToFillForm.Where(w => formDetail.Where(w => Convert.ToInt32(w.Title) == Id).Select(s => s.Id).Contains(Convert.ToInt32(w.DetailId))))
                {
                    foreach (var item2 in needToFillForm.Where(w => formDetail.Where(w => Convert.ToInt32(w.LowerTitle) == item.DetailId).Select(s => s.Id).Contains(w.DetailId)))
                    {
                        decTextLvl0 += Convert.ToDouble(item2.GivenPoint) * formDetail.Where(w => w.Id == item2.DetailId).Select(s => Convert.ToDouble(s.Weight)).FirstOrDefault() / 100.0;
                    }
                    result += Math.Round(decTextLvl0 * formDetail.Where(w => w.Id == item.DetailId).Select(s => Convert.ToDouble(s.Weight)).FirstOrDefault() / 100.0, 2);
                    decTextLvl0 = 0;
                }
                result = Math.Round(result * formDetail.Where(w => w.Id == Id).Select(s => Convert.ToDouble(s.Weight)).FirstOrDefault() / 100.0, 2);
                result = Math.Round(result / 5 * 100.0, 2);
            }
            return result;
        }
    }
}
