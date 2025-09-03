using MaxJobTracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace MaxJobTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApplicationUserController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetUsers2()
        {
            return Ok(MaxJobTracker.Data.DummyUserStore.UserList);
        }

        [HttpGet("getUser")] // You can name the route specifically here
        public IActionResult GetUsers()
        {
            try
            {
             
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filePath = Path.Combine(wwwRootPath, "js", "UserDummyData.json");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Dummy data file not found.");
                }
                var jsonString = System.IO.File.ReadAllText(filePath);

                return Content(jsonString, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery] string name)
        {
 
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("A name must be provided for searching.");
            }

            try
            {
              
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "js", "UserDummyData.json");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Dummy data file not found.");
                }
                var jsonString = System.IO.File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var userList = JsonSerializer.Deserialize<List<ApplicationUser>>(jsonString, options);
                var foundUsers = userList.Where(user =>
                    user.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                if (foundUsers.Any())
                {
                    return Ok(foundUsers); // Return the list of matching users
                }
                else
                {
                    return NotFound($"No user found with the name '{name}'.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }


    }
}
