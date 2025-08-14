namespace Domain
{
    public abstract class Exercise
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        protected Exercise(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }
    }
}