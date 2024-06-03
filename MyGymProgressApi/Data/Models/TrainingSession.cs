using backend.Data.Models;

namespace MyGymProgress.Data.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string WorkoutName { get; set; }
        public string? Notes { get; set; }
        public string? WorkoutNotes { get; set; }
        public string WorkoutDuration { get; set; }

        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

    }
}
