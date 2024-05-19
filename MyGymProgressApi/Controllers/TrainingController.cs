//using Amazon.RDS;
//using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyGymProgress.Data;
using MyGymProgress.Data.Models;
using System.Linq;
using backend.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using MyGymProgressApi.Handlers.CsvHandler.Records;
using CsvHelper.Configuration;

namespace MyGymProgressApi.Controllers
{
  
    //[ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ILogger<TrainingController> _logger;
        //private readonly IAmazonS3 _amazonS3;
        //private readonly IAmazonRDS _amazonRDS;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _appDbContext;

        public TrainingController(ILogger<TrainingController> logger,
            //IAmazonS3 amazonS3,
            IHttpClientFactory httpClientFactory,
            //IAmazonRDS amazonRDS,
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: api/retrieveTrainingSessions
        //[HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetAll()
        {
            Console.WriteLine("Getting All Training Sessions");
            return await _appDbContext.TrainingSessions.Include(ts => ts.Exercises).ToListAsync();
        }

        // GET: api/Training/5
        //[HttpGet("{id}")]
        public async Task<ActionResult<TrainingSession>> Get(int id)
        {
            var training = await _appDbContext.TrainingSessions.Include(ts => ts.Exercises).FirstOrDefaultAsync(ts => ts.Id == id);
            if (training == null)
            {
                return NotFound();
            }
            return training;
        }

        // POST: api/sendTrainingSessions
        public async Task<IActionResult> PostAll(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded...");
            }

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HeaderValidated = null,
                Delimiter = ";",
                IgnoreBlankLines = true,
                PrepareHeaderForMatch = args => args.Header.Replace(" ", "")
            };

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var exerciseRecords = csv.GetRecords<ExerciseCsv>().ToList();
                    var groupedSessions = exerciseRecords
                        .GroupBy(e => new { e.Date, e.WorkoutName })
                        .Select(g => new TrainingSession
                        {
                            Date = ParseDateAsUtc(g.Key.Date),
                            WorkoutName = g.Key.WorkoutName,
                            Notes = g.First().Notes, //Assumes first record's notes are session notes
                            WorkoutNotes = g.First().WorkoutNotes,
                            WorkoutDuration = g.First().WorkoutDuration,
                            Exercises = g.Select(e => new Exercise
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
                        }).ToList();
try
                    {
                    await _appDbContext.AddRangeAsync(groupedSessions);
                    await _appDbContext.SaveChangesAsync();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }

                return Ok("Training sessions uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing the file: {ex.Message}");
            }
        }


        private List<TrainingSession> GroupAndCreateSessions(IEnumerable<ExerciseCsv> records)
        {
            return records
                .GroupBy(r => new { r.Date, r.WorkoutName })
                .Select(g => new TrainingSession
                {
                    Date = g.Key.Date,
                    WorkoutName = g.Key.WorkoutName,
                    Notes = g.First().Notes,
                    WorkoutNotes = g.First().WorkoutNotes,
                    WorkoutDuration = g.First().WorkoutDuration,
                    Exercises = g.Select(e => new Exercise
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
                }).ToList();
        }
        private bool TrainingExists(int id)
        {
            return _appDbContext.TrainingSessions.Any(e => e.Id == id);
        }

        public DateTime ParseDateAsUtc(DateTime date)
        {
            //TimeZoneInfo melbourneTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            

            //// Ensure the date kind is specified as Local
            //if (date.Kind == DateTimeKind.Unspecified)
            //{
            try
            {
Console.WriteLine($"Original Date: {date}, Kind: {date.Kind}");
                var localDate = DateTime.SpecifyKind(date, DateTimeKind.Local);
                var utcDate = localDate.ToUniversalTime();
                return utcDate;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(date);
                
            }
            return date;
            
                //Console.WriteLine($"Specified Date as Local: {date}, Kind: {date.Kind}");
            //}

            //// Convert the date to UTC using Melbourne time zone
            //try
            //{
            //    var utcDate = TimeZoneInfo.ConvertTimeToUtc(date, melbourneTimeZone);
            //    Console.WriteLine($"Converted Date to UTC: {utcDate}, Kind: {utcDate.Kind}");
            //    return utcDate;
            //}
            //catch (ArgumentException ex)
            //{
            //    Console.WriteLine($"Error during conversion: {ex.Message}");
            //    throw; // Re-throw the exception to handle it further up the call stack
            //}
        }





    }
}
