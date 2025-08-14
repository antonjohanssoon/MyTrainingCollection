namespace Domain
{
    public class CardioExercise : Exercise
    {
        public double DistanceKm { get; set; }
        public TimeSpan Duration { get; set; }

        public CardioExercise(string name, double distanceKm, TimeSpan duration) : base(name)
        {
            DistanceKm = distanceKm;
            Duration = duration;
        }
    }
}
