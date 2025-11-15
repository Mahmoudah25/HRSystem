using HRManagmentSystem.Models.Traning_Performance;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRManagmentSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary {  get; set; }
        public DateTime HireDate { get; set; }
        public bool IsDeleted { set; get; } = false;


        //Fks
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        [ForeignKey("Position")]
        public int PositionId {  get; set; }
        [JsonIgnore]
        public Position Position { get; set; }
        
        //RelationShips
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Leave> Leends { get; set; } = new HashSet<Leave>();
        public ICollection<Payrolls> Payrolls { get; set; } = new List<Payrolls>();
        public ICollection<OverTime> OverTime { get; set; } = new List<OverTime>();
        public ICollection<Allowwance> Allowwances { get; set;} = new HashSet<Allowwance>();
        public ICollection<Deduction> Deductions { get; set; } = new HashSet<Deduction>(); 
        public ICollection<EmployeeTraning> EmployeeTranings { get; set; } = new List<EmployeeTraning>();
        public ICollection<PerformanceReview> PerformanceReviews { get; set;} = new HashSet<PerformanceReview>();

        //One-to-one
        public ApplicationUser ApplicationUser { get; set; }    

    }
}
