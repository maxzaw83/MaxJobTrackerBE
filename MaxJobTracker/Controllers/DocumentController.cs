using MaxJobTracker.Data;
using MaxJobTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaxJobTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentController (ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env; 
        }

        // POST: api/documents/upload/{jobApplicationId}
     
        [HttpPost("{jobApplicationId}")]
        public async Task<IActionResult> UploadDocument(int jobApplicationId,  IFormFile file)
        {
            var jobApplication = await _context.JobApplications.FindAsync(jobApplicationId);
            if (jobApplication == null) return NotFound("Job application not found.");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var uploadsFolderPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                FileName = file.FileName,
                FilePath = Path.Combine("uploads", uniqueFileName), // Store relative path
                ContentType = file.ContentType,
                JobApplicationId = jobApplicationId
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(document);
        }

        // GET: api/documents/download/5
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, document.FilePath);
            if (!System.IO.File.Exists(filePath)) return NotFound("File not found on server.");

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, document.ContentType, document.FileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Delete the physical file from the server
            var filePath = Path.Combine(_env.WebRootPath, document.FilePath);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Delete the record from the database
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return NoContent(); // Success, no content to return
        }

    }
}
