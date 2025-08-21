using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public enum ExerciseType { Strength, Cardio }
    public class ExerciseDTO
    {
        public Guid? Id { get; set; }
        public ExerciseType Type { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public string Name { get; set; }
        //Strength
        public int? Sets { get; set; }
        public int? Reps { get; set; }
        public double? Weight { get; set; }
        //Cardio
        public double? DistanceKm { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}
