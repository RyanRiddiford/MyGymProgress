using backend.Data.Models;
using MyGymProgress.Data.Models;

namespace MyGymProgressApi.Handlers.CsvHandler.Records
{
    /// <summary>
    /// Represents a training session as imported from a CSV file, including all related exercises.
    /// </summary>
    public class TrainingSessionCsv
    {
        public DateTime Date { get; set; }
        public string WorkoutName { get; set; }
        public string Notes { get; set; }
        public string WorkoutNotes { get; set; }
        public double WorkoutDuration { get; set; }
        public List<ExerciseCsv> Exercises { get; set; }

        //Method to convert to domain model
        public TrainingSession ToTrainingSession()
        {
            return new TrainingSession
            {
                Date = this.Date,
                WorkoutName = this.WorkoutName,
                Notes = this.Notes,
                WorkoutNotes = this.WorkoutNotes,
                WorkoutDuration = WorkoutDuration.ToString(),
                Exercises = Exercises.Select(e => new Exercise
                {
                    ExerciseName = e.ExerciseName,
                    SetOrder = e.SetOrder,
                    Weight = e.Weight,
                    WeightUnit = e.WeightUnit,
                    Reps = e.Reps,
                    RPE = e.RPE,
                    Distance = e.Distance,
                    DistanceUnit = e.DistanceUnit,
                    Seconds = e.Seconds
                }).ToList()
            };
        }
    }

}
