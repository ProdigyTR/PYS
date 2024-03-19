using PerformansYonetimSistemi.Helper.Database;
using PerformansYonetimSistemi.Models.Defination;
using System.Text.RegularExpressions;

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
                text = text.Replace(karakterler[i],orjinaller[i]);
            }
            return text;
        }
        public string ResponsibleList(string tcNumbers,List<Employee> employeeModel)
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
    }
}
