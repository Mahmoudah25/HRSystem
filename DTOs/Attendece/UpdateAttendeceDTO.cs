namespace HRManagmentSystem.DTOs.Attendece
{
    public class UpdateAttendeceDTO
    {
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string? Status { get; set; }

    }
}
