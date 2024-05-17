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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Define the PostgreSQL schema for the TrainingSession entity
            modelBuilder.Entity<TrainingSession>();
        }
    }
}