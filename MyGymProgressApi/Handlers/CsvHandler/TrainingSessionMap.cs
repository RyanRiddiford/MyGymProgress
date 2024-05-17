//using CsvHelper.Configuration;
//using MyGymProgressApi.Handlers.CsvHandler.Records;

//public class TrainingSessionMap : ClassMap<TrainingSessionCsv>
//{
//    public TrainingSessionMap()
//    {
//        Map(m => m.Date).Name("Date");
//        Map(m => m.WorkoutName).Name("WorkoutName");
//        Map(m => m.Notes).Name("Notes");
//        Map(m => m.WorkoutNotes).Name("WorkoutNotes");
//        Map(m => m.WorkoutDuration).Name("WorkoutDuration");

//        // ExerciseCsvRecord needs to be properly defined in your context
//        Map(m => m.Exercises).ConvertUsing(row =>
//        {
//            var exercises = new List<ExerciseCsvRecord>();
//            int i = 1;
//            while (true)  // Assuming there is no fixed number of exercises
//            {
//                try
//                {
//                    // Check if the first required field for a new Exercise exists
//                    if (string.IsNullOrEmpty(row.GetField<string>($"Exercise{i}Name")))
//                        break;

//                    exercises.Add(new ExerciseCsvRecord
//                    {
//                        ExerciseName = row.GetField<string>($"Exercise{i}Name"),
//                        SetOrder = row.GetField<int>($"Exercise{i}SetOrder"),
//                        Weight = row.GetField<decimal>($"Exercise{i}Weight"),
//                        WeightUnit = row.GetField<string>($"Exercise{i}WeightUnit"),
//                        Reps = row.GetField<int>($"Exercise{i}Reps"),
//                        RPE = row.GetField<int>($"Exercise{i}RPE"),
//                        Distance = row.GetField<decimal>($"Exercise{i}Distance"),
//                        DistanceUnit = row.GetField<string>($"Exercise{i}DistanceUnit"),
//                        Seconds = row.GetField<int>($"Exercise{i}Seconds")
//                    });
//                    i++;
//                }
//                catch (CsvHelperException)
//                {
//                    // Break if any field does not exist indicating end of data
//                    break;
//                }
//            }
//            return exercises;
//        });
//    }
//}
