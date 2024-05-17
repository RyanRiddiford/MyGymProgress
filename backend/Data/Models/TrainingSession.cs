using System;
using System.ComponentModel.DataAnnotations;

namespace MyGymProgress.Data.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Workout name is required")]
        public string? WorkoutName { get; set; }

        [Required(ErrorMessage = "Exercise name is required")]
        public string? ExerciseName { get; set; }

        public int SetOrder { get; set; }

        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Weight unit is required")]
        public string? WeightUnit { get; set; }

        public int Reps { get; set; }

        public int RPE { get; set; }

        public decimal Distance { get; set; }

        [Required(ErrorMessage = "Distance unit is required")]
        public string? DistanceUnit { get; set; }

        public int Seconds { get; set; }

        [Required(ErrorMessage = "Notes are required")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Workout notes are required")]
        public string? WorkoutNotes { get; set; }

        [Required(ErrorMessage = "Workout duration is required")]
        public string? WorkoutDuration { get; set; }
    }
}
