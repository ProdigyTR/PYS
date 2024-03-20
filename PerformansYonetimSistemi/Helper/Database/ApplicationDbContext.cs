using PerformansYonetimSistemi.Models.Login;
using PerformansYonetimSistemi.Models.HR;
using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Models.Defination;

namespace PerformansYonetimSistemi.Helper.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<IK_User> IK_Users { get; set; }
        public DbSet<FormMas> FormMas { get; set; }
        public DbSet<FormDetail> FormDetails { get; set; }
        public DbSet<NeedToFillForm> NeedToFillForms { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<KPI> KPIs { get; set; }
        public DbSet<Target> Targets { get; set; }
        public DbSet<PerformanceCard> PerformanceCards { get; set; }
        public DbSet<EmployeeKpi> EmployeeKpis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IK_User>().ToTable("IK_User");
            modelBuilder.Entity<FormMas>().ToTable("FormMas");
            modelBuilder.Entity<FormDetail>().ToTable("FormDetail");
            modelBuilder.Entity<NeedToFillForm>().ToTable("NeedToFillForm");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Position>().ToTable("Position");
            modelBuilder.Entity<KPI>().ToTable("KPI");
            modelBuilder.Entity<Target>().ToTable("Target");
            modelBuilder.Entity<PerformanceCard>().ToTable("PerformanceCard");
            modelBuilder.Entity<EmployeeKpi>().ToTable("EmployeeKpi");
        }      
    }
}


