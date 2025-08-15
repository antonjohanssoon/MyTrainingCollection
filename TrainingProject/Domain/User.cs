namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();

        public User(string name, string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Name = name;
            Username = username;
            PasswordHash = passwordHash;
        }

        public User()
        {

        }
    }
}
