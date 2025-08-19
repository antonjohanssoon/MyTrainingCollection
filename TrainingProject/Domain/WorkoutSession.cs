namespace Domain
{
    public class WorkoutSession
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();

        public WorkoutSession(DateTime date, User user, string notes = "")
        {
            Id = Guid.NewGuid();
            Date = date;
            User = user;
            UserId = user.Id;
            Notes = notes;
        }

        public WorkoutSession()
        {

        }
    }
}