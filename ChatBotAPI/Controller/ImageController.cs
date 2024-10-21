using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResponseController : ControllerBase
    {
        // Other existing code...

        // Endpoint to return an image
        [HttpGet("image/{imageName}")]
        public async Task<IActionResult> GetImage(string imageName)
        {
            // Define the path to your image directory
            var imagePath = Path.Combine("Path/To/Your/Images", imageName);

            // Check if the file exists
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Image not found.");
            }

            // Read the image file
            var imageFile = await System.IO.File.ReadAllBytesAsync(imagePath);

            // Return the image as a FileResult
            return File(imageFile, "image/jpeg"); // Change the content type if your image is not JPEG
        }
    }
}
