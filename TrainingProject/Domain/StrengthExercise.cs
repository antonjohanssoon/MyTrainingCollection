namespace Domain
{
    public class StrengthExercise : Exercise
    {

        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Weight { get; set; }

        public StrengthExercise(string name, int sets, int reps, double weight) : base(name)
        {
            Sets = sets;
            Reps = reps;
            Weight = weight;
        }
    }
}
