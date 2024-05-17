using backend.Data.Models;
using Microsoft.EntityFrameworkCore;
using MyGymProgress.Data.Models;

namespace MyGymProgress.Data
{


    /// <summary>
    /// Database Migrations schematics builder
    /// </summary>
    public class AppDbContext : DbContext
    {

        public AppDbContext()
        { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TrainingSession>()
    .HasMany(s => s.Exercises)
    .WithOne(e => e.TrainingSession)
    .HasForeignKey(e => e.TrainingSessionId);

            modelBuilder.Entity<TrainingSession>().HasData(
                new TrainingSession
                {
                    Id = 1,
                    Date = DateTime.Now.ToUniversalTime(),
                    WorkoutName = "Full Body Routine",
                    Notes = "Full body workout focusing on form",
                    WorkoutNotes = "Felt good today, no pain.",
                    WorkoutDuration = "1 hour"
                });

            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    TrainingSessionId = 1,
                    ExerciseName = "Deadlift",
                    SetOrder = 1,
                    Weight = 120m,
                    WeightUnit = "kg",
                    Reps = 5,
                    RPE = 8,
                    Distance = 0m,
                    DistanceUnit = "m",  // Ensure this has a value
                    Seconds = 0
                },
                new Exercise
                {
                    Id = 2,
                    TrainingSessionId = 1,
                    ExerciseName = "Squat",
                    SetOrder = 2,
                    Weight = 100m,
                    WeightUnit = "kg",
                    Reps = 5,
                    RPE = 8,
                    Distance = 0m,
                    DistanceUnit = "m",  // Ensure this has a value
                    Seconds = 0
                }
            );



        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=localdb;Username=postgres;");
            }
        }

    }
}