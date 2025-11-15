using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models.Traning_Performance
{
    public class TrainingCertificate
    {
        public int Id { get; set; }
        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public string CertificateName { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }

        public Traning Training { get; set; }
        public Employee Employee { get; set; }

    }
}
