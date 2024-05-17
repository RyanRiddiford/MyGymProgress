//using Amazon.RDS;
//using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyGymProgress.Data.Models;
using MyGymProgress.Data;

namespace MyGymProgress.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ILogger<TrainingController> _logger;
        //private readonly IAmazonS3 _amazonS3;
        //private readonly IAmazonRDS _amazonRDS;
        private readonly AppDbContext _appDbContext;

        public TrainingController(ILogger<TrainingController> logger,
            //IAmazonS3 amazonS3,
            IHttpClientFactory httpClientFactory,
            //IAmazonRDS amazonRDS,
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // GET: api/Training
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingSession>>> GetTrainings()
        {
            return await _appDbContext.TrainingSessions.ToListAsync();
        }

        // GET: api/Training/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingSession>> GetTraining(int id)
        {
            var training = await _appDbContext.TrainingSessions.FindAsync(id);

            if (training == null)
            {
                return NotFound();
            }

            return training;
        }

        // PUT: api/Training/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTraining(int id, TrainingSession trainingSession)
        {
            if (id != trainingSession.Id)
            {
                return BadRequest();
            }

            _appDbContext.Entry(trainingSession).State = EntityState.Modified;

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool TrainingExists(int id)
        {
            return _appDbContext.TrainingSessions.Any(e => e.Id == id);
        }
    }
}
