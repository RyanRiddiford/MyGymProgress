

namespace MyGymProgress.Routes
{
    public static class TrainingRoutes
    {
        public static void MapTrainingRoutes(this IEndpointRouteBuilder endpoints)
        {
            //GET: All Training Sessions
            endpoints.MapControllerRoute(
                name: "GET",
                pattern: "api/retrieveTrainingSessions",
                defaults: new { controller = "Training", action = "GetAll" }
            );
            //GET: One Training Session
            endpoints.MapControllerRoute(
                name: "GET",
                pattern: "api/trainingSession/{id}",
                defaults: new { controller = "Training", action = "Get" }
            );
            //GET: Page of Training Sessions
            endpoints.MapControllerRoute(
                name: "GET",
                pattern: "api/trainingSession/page/{page}",
                defaults: new { controller = "Training", action = "GetPage" }
            );
            //GET: Number of Training Session pages
            endpoints.MapControllerRoute(
                name: "GET",
                pattern: "api/trainingSession/numPages",
                defaults: new { controller = "Training", action = "GetNumPages" }
                );
            //POST: All Training Sessions
            endpoints.MapControllerRoute(
                name: "POST",
                pattern: "api/sendTrainingSessions",
                defaults: new { controller = "Training", action = "PostAll" }
             );
        }
    }
}
