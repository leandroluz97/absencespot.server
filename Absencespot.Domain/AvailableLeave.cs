namespace Absencespot.Domain
{
    public class AvailableLeave
    {
        public double AvailableDays { get; set; }
        public DateTime Period { get; set; }
        public int? AbsenceId { get; set; }
        public Absence? Absence { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
