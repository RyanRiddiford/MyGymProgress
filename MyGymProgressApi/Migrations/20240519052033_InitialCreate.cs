using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyGymProgressApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WorkoutName = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    WorkoutNotes = table.Column<string>(type: "text", nullable: true),
                    WorkoutDuration = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TrainingSessionId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseName = table.Column<string>(type: "text", nullable: true),
                    SetOrder = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightUnit = table.Column<string>(type: "text", nullable: true),
                    Reps = table.Column<int>(type: "integer", nullable: true),
                    RPE = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<decimal>(type: "numeric", nullable: true),
                    DistanceUnit = table.Column<string>(type: "text", nullable: true),
                    Seconds = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TrainingSessions",
                columns: new[] { "Id", "Date", "Notes", "WorkoutDuration", "WorkoutName", "WorkoutNotes" },
                values: new object[] { 1, new DateTime(2024, 5, 19, 5, 20, 33, 263, DateTimeKind.Utc).AddTicks(5720), "Full body workout focusing on form", "1 hour", "Full Body Routine", "Felt good today, no pain." });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "Distance", "DistanceUnit", "ExerciseName", "RPE", "Reps", "Seconds", "SetOrder", "TrainingSessionId", "Weight", "WeightUnit" },
                values: new object[,]
                {
                    { 1, 0m, "m", "Deadlift", 8, 5, 0, 1, 1, 120m, "kg" },
                    { 2, 0m, "m", "Squat", 8, 5, 0, 2, 1, 100m, "kg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TrainingSessionId",
                table: "Exercises",
                column: "TrainingSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "TrainingSessions");
        }
    }
}
