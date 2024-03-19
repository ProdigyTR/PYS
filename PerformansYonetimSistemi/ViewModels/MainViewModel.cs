using PerformansYonetimSistemi.Models.Defination;
using PerformansYonetimSistemi.Models.HR;
using PerformansYonetimSistemi.Models.Login;

namespace PerformansYonetimSistemi.ViewModels
{
    public class MainViewModel
    {
        public List<IK_User> IK_Users { get; set; }
        public List<FormMas> FormMases { get; set; }
        public List<FormDetail> FormDetails { get; set; }
        public List<NeedToFillForm> NeedToFillForms { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Department> Departments { get; set; }
        public List<Position> Positions { get; set; }
        public List<KPI> KPIs { get; set; }
        public List<Target> Targets { get; set; }
        public List<KpiScoreCard> KpiScoreCards { get; set; }
        public List<EmployeeKpi> EmployeeKpis { get; set; }
    }
}
