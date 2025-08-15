namespace Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<WorkoutSession> WorkoutSessions { get; set; } = new();

        public User(string name, string username, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Username = username;
            Password = password;
        }
    }
}
