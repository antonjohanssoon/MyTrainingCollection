namespace Domain
{
    public class WorkoutSession
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public List<Exercise> Exercises { get; set; } = new();
        public string Notes { get; set; }

        public WorkoutSession(DateTime date, string notes = "")
        {
            Id = Guid.NewGuid();
            Date = date;
            Notes = notes;
        }
    }
}