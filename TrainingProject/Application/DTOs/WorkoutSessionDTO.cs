namespace Application.DTOs
{
    public class WorkoutSessionDTO
    {
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public List<ExerciseDTO> Exercises { get; set; }
    }
}
