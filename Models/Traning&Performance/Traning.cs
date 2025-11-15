namespace HRManagmentSystem.Models.Traning_Performance
{
    public enum TrainigStatus
    {
        Scheduling,
        InProgress,
        Finished
    }
    public class Traning
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }

        //ReationShips
        public TrainigStatus Status { set; get; } = TrainigStatus.Scheduling;
        public ICollection<EmployeeTraning> EmployeeTranings { get; set; } = new List<EmployeeTraning>();
    }
}
