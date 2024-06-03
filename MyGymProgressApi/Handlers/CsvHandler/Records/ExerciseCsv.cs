public class ExerciseCsv
{
    public DateTime Date { get; set; }
    public string WorkoutName { get; set; }
    public string ExerciseName { get; set; }
    public int SetOrder { get; set; }
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public int? Reps { get; set; }
    public int? RPE { get; set; }
    public decimal? Distance { get; set; }
    public string? DistanceUnit { get; set; }
    public int? Seconds { get; set; }
    public string? Notes { get; set; }
    public string? WorkoutNotes { get; set; }
    public string WorkoutDuration { get; set; }
}
