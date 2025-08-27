using MaxJobTracker.Data;
using MaxJobTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaxJobTracker.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public JobApplicationController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetJobApplications()
        {
            return await _context.JobApplications.Include(ja => ja.Documents).ToListAsync();
        }

        // GET: api/jobapplication/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplication>> GetJobApplication(int id)
        {
            var jobApplication = await _context.JobApplications.Include(ja => ja.Documents)
                .FirstOrDefaultAsync(ja => ja.Id == id);

            if (jobApplication == null)
            {
                return NotFound();
            }

            return jobApplication;
        }

        // POST: api/jobapplications
        [HttpPost]
        public async Task<ActionResult<JobApplication>> PostJobApplication(JobApplication jobApplication)
        {
            jobApplication.DateSaved = DateTime.UtcNow;
            _context.JobApplications.Add(jobApplication);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJobApplication), new { id = jobApplication.Id }, jobApplication);
        }

        // --- ADD THIS UPDATE METHOD ---
        // PUT: api/jobapplications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobApplication(int id, JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return BadRequest();
            }

            // Detach the existing entity if it's being tracked to avoid conflicts.
            var local = _context.Set<JobApplication>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(jobApplication).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JobApplications.Any(e => e.Id == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobApplication(int id)
        {
            var jobApplication = await _context.JobApplications.Include(ja => ja.Documents).FirstOrDefaultAsync(ja => ja.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            // Also delete associated files from the server's file system
            foreach (var doc in jobApplication.Documents)
            {
                var filePath = Path.Combine(_env.WebRootPath, doc.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.JobApplications.Remove(jobApplication);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}


