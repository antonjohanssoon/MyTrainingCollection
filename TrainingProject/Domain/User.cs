namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<WorkoutSession> WorkoutSessions { get; set; } = new();

        public User(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}
