namespace HRManagmentSystem.Models.Communication
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CraeteAt { get; set; }
        public bool IsDeleted { set; get; } = false;

    }
}
