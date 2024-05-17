using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace MyGymProgress.Routes
{
    public static class TrainingRoutes
    {
        public static void MapTrainingRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "GetTrainings",
                pattern: "api/trainings",
                defaults: new { controller = "Training", action = "GetTrainings" }
            );

            endpoints.MapControllerRoute(
                name: "GetTraining",
                pattern: "api/training/{id}",
                defaults: new { controller = "Training", action = "GetTraining" }
            );

            // Add more routes as needed
        }
    }
}
