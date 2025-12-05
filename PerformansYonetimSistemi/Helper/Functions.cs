using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.Models.HR;
using Microsoft.EntityFrameworkCore;

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
        public decimal CalculateTotalPoint(List<FormDetail> formDetail, List<Competency> competencys)
        {
            decimal result = 0;
            decimal dblTxtLvl0 = 0;
            foreach (var txtLvl0 in competencys.Where(w => w.TextLevel == 0).OrderBy(o => o.DetailId))
            {
                decimal dblTxtLvl1 = 0;
                foreach (var txtLvl1 in competencys.Where(w => w.TextLevel == 1 && w.Title == txtLvl0.DetailId.ToString()).OrderBy(o => o.DetailId))
                {
                    decimal dblTxtLvl2 = 0;
                    foreach (var txtLvl2 in competencys.Where(w => w.TextLevel == 2 && w.LowerTitle == txtLvl1.DetailId.ToString()).OrderBy(o => o.DetailId))
                    {
                        dblTxtLvl2 = dblTxtLvl2 + Convert.ToDecimal(txtLvl2.GivenPoint.Replace(".",",")) * formDetail.Where(w => w.Id == txtLvl2.DetailId).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault() / Convert.ToDecimal(100);
                    }
                    dblTxtLvl1 = dblTxtLvl1 + dblTxtLvl2 * formDetail.Where(w => w.Id == txtLvl1.DetailId).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault() / Convert.ToDecimal(100);
                }
                dblTxtLvl0 = dblTxtLvl0 + dblTxtLvl1 * formDetail.Where(w => w.Id == txtLvl0.DetailId).Select(s => s.Weight).FirstOrDefault();
            }
            result = dblTxtLvl0;
            return result;
        }
        public decimal CalculatePoint(int Id, List<FormDetail> formDetail, List<Competency> competencys)
        {
            decimal result = 0;

            int textLvl = formDetail.Where(w => w.Id == Id).Select(s => s.TextLevel).FirstOrDefault();

            decimal decTextLvl0 = 0;
            decimal decTextLvl1 = 0;

            if (textLvl == 1)
            {
                foreach (var item in competencys.Where(w => formDetail.Where(w => Convert.ToInt32(w.LowerTitle) == Id).Select(s => s.Id).Contains(w.DetailId)))
                {
                    decTextLvl1 += Convert.ToDecimal(item.GivenPoint.Replace(".",",")) * formDetail.Where(w => w.Id == item.DetailId).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault()/ Convert.ToDecimal(100.0);
                }
                result = Math.Round(decTextLvl1, 2);
                
            }
            else if (textLvl == 0)
            {
                foreach (var item in competencys.Where(w => formDetail.Where(w => Convert.ToInt32(w.Title) == Id).Select(s => s.Id).Contains(Convert.ToInt32(w.DetailId))))
                {
                    foreach (var item2 in competencys.Where(w => formDetail.Where(w => Convert.ToInt32(w.LowerTitle) == item.DetailId).Select(s => s.Id).Contains(w.DetailId)))
                    {
                        decTextLvl0 += Convert.ToDecimal(item2.GivenPoint.Replace(".",",")) * formDetail.Where(w => w.Id == item2.DetailId).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault() / Convert.ToDecimal(100.0);
                    }
                    result += Math.Round(decTextLvl0 * formDetail.Where(w => w.Id == item.DetailId).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault() / Convert.ToDecimal(100.0), 4);
                    decTextLvl0 = 0;
                }
                //result = Math.Round(result * formDetail.Where(w => w.Id == Id).Select(s => Convert.ToDecimal(s.Weight)).FirstOrDefault(), 2);
            }
            return Math.Round(result,2);
        }
    }
}
