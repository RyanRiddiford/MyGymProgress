/**
 * Contains actions for Training Sessions
 */

using backend.Data.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGymProgress.Data;
using MyGymProgress.Data.Models;
using System.Globalization;

namespace MyGymProgressApi.Controllers
{
    /// <summary>
    /// Handles actions for training sessions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingController : ControllerBase
    {
        private readonly ILogger<TrainingController> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _appDbContext;
        private static readonly Dictionary<string, List<byte[]>> fileChunks = new Dictionary<string, List<byte[]>>();
        private readonly IWebHostEnvironment _environment;
        private readonly byte PAGE_SIZE = 10;

        public TrainingController(ILogger<TrainingController> logger,
            IHttpClientFactory httpClientFactory,
            AppDbContext appDbContext,
            IWebHostEnvironment environment)
        {
            _appDbContext = appDbContext;
            _httpClient = httpClientFactory.CreateClient();
            _environment = environment;
        }

        /// <summary>
        /// Retrieves all training sessions with associated exercises.
        /// </summary>
        /// <returns>A list of training sessions.</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetAll()
        {
            Console.WriteLine("Getting All Training Sessions");
            return await _appDbContext.TrainingSessions.Include(ts => ts.Exercises).ToListAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of training sessions.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <returns>A list of training sessions for the specified page.</returns>
        [HttpGet("page/{page}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetPage(int page)
        {
            Console.WriteLine("Getting page of Training Sessions");
            return await _appDbContext.TrainingSessions
                .Include(ts => ts.Exercises)
                .OrderBy(ts => ts.Date)
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves the number of pages of training sessions.
        /// </summary>
        /// <returns>The total number of pages.</returns>
        [HttpGet("numPages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNumPages()
        {
            Console.WriteLine($"{nameof(GetNumPages)}");
            int count = await _appDbContext.TrainingSessions.CountAsync();
            return Ok(count);
        }

        /// <summary>
        /// Retrieves a specific training session by ID.
        /// </summary>
        /// <param name="id">The ID of the training session to retrieve.</param>
        /// <returns>The training session if found; otherwise, NotFound.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TrainingSession>> GetByID(int id)
        {
            var training = await _appDbContext.TrainingSessions.Include(ts => ts.Exercises).FirstOrDefaultAsync(ts => ts.Id == id);
            if (training == null)
            {
                return NotFound();
            }
            return training;
        }

        /// <summary>
        /// Uploads training sessions in chunks.
        /// </summary>
        /// <param name="chunk">The file chunk to upload.</param>
        /// <param name="fileId">The file identifier for the chunks.</param>
        /// <param name="chunkNumber">The current chunk number.</param>
        /// <param name="totalChunks">The total number of chunks.</param>
        /// <returns>Status of the upload process.</returns>
        [HttpPost("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAll(IFormFile chunk, [FromForm] string fileId, [FromForm] int chunkNumber, [FromForm] int totalChunks)
        {
            if (chunk == null || chunk.Length == 0)
            {
                return BadRequest("No chunk uploaded...");
            }

            if (!fileChunks.ContainsKey(fileId))
            {
                fileChunks[fileId] = new List<byte[]>();
            }

            using (var memoryStream = new MemoryStream())
            {
                await chunk.CopyToAsync(memoryStream);
                fileChunks[fileId].Add(memoryStream.ToArray());
            }

            if (fileChunks[fileId].Count == totalChunks)
            {
                var completeFilePath = Path.Combine(_environment.ContentRootPath, "uploads", $"{fileId}.csv");
                using (var fileStream = new FileStream(completeFilePath, FileMode.Create))
                {
                    foreach (var fileChunk in fileChunks[fileId])
                    {
                        await fileStream.WriteAsync(fileChunk, 0, fileChunk.Length);
                    }
                }

                fileChunks.Remove(fileId);
                return await ProcessCompleteFile(completeFilePath);
            }

            return Ok("Chunk uploaded successfully.");
        }

        [NonAction]
        private async Task<IActionResult> ProcessCompleteFile(string filePath)
        {
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
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var exerciseRecords = csv.GetRecords<ExerciseCsv>().ToList();
                    var groupedSessions = exerciseRecords
                        .GroupBy(e => new { e.Date, e.WorkoutName })
                        .Select(g => new TrainingSession
                        {
                            Date = ParseDateAsUtc(g.Key.Date),
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

                    await _appDbContext.AddRangeAsync(groupedSessions);
                    await _appDbContext.SaveChangesAsync();
                }

                return Ok("Training sessions uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing the file: {ex.Message}");
            }
        }

        [NonAction]
        public DateTime ParseDateAsUtc(DateTime date)
        {
            try
            {
                Console.WriteLine($"Original Date: {date}, Kind: {date.Kind}");
                var localDate = DateTime.SpecifyKind(date, DateTimeKind.Local);
                var utcDate = localDate.ToUniversalTime();
                return utcDate;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(date);

            }
            return date;
        }






    }
}




