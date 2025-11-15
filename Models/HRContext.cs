using HRManagmentSystem.Models.Communication;
using HRManagmentSystem.Models.Traning_Performance;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRManagmentSystem.Models
{
    public class HRContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        // DbSets
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<Payrolls> Payrolls { get; set; }
        public DbSet<OverTime> Overtimes { get; set; }
        public DbSet<Allowwance> Allowances { get; set; }
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<Traning> Trainings { get; set; }
        public DbSet<EmployeeTraning> EmployeeTrainings { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<TrainingCertificate> trainingCertificates { get; set; }

        public HRContext(DbContextOptions<HRContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-One: Employee ↔ User
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithOne(u => u.Employee)
                .HasForeignKey<ApplicationUser>(u => u.EmployeeId);

            // Many-to-Many: Employee ↔ Training
            modelBuilder.Entity<EmployeeTraning>()
                .HasKey(et => new { et.EmployeeId, et.TraningId });

            modelBuilder.Entity<Employee>()
      .Property(e => e.Salary)
      .HasPrecision(18, 2);

            modelBuilder.Entity<Allowwance>()
                .Property(a => a.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Deduction>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OverTime>()
                .Property(o => o.Rate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payrolls>()
                .Property(p => p.Allowances)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payrolls>()
                .Property(p => p.BaseSalary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payrolls>()
                .Property(p => p.Deductions)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payrolls>()
                .Property(p => p.NetSalary)
                .HasPrecision(18, 2);

        }
    }
}