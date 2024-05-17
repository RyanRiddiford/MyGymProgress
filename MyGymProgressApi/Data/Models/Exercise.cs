using MyGymProgress.Data.Models;

namespace backend.Data.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public int TrainingSessionId { get; set; }
        public string? ExerciseName { get; set; }
        public int SetOrder { get; set; }
        public decimal? Weight { get; set; }
        public string? WeightUnit { get; set; }
        public int? Reps { get; set; }
        public int? RPE { get; set; }
        public decimal? Distance { get; set; }
        public string? DistanceUnit { get; set; }
        public int? Seconds { get; set; }

        public TrainingSession? TrainingSession { get; set; }
    }
}
