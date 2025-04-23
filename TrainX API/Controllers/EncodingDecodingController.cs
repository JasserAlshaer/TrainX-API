using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrainX_API.Helpers;
using static TrainX_API.Helpers.AesEncryptionService;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncodingDecodingController : ControllerBase
    {
        [HttpPost("encode")]
        public async Task<IActionResult> EncodeFile([FromForm] IFormFile file, [FromForm] FileType fileType)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy file contents to memory stream
                    await file.CopyToAsync(memoryStream);

                    // Convert to Base64 string
                    string base64String = Convert.ToBase64String(memoryStream.ToArray());

                    // Return the Base64 string along with file type
                    return Ok(new { FileType = fileType.ToString(), Base64Data = base64String });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("decode")]
        public async Task<IActionResult> DecodeFile([FromBody] FileBase64Request request)
        {
            if (string.IsNullOrEmpty(request.Base64Data))
                return BadRequest("Base64 data is required.");

            try
            {
                // Decode the Base64 string to byte array
                byte[] fileBytes = Convert.FromBase64String(request.Base64Data);

                // Define a file path (you can dynamically generate this based on the file type or file name)
                var filePath = Path.Combine("Attachments", $"{Guid.NewGuid()}.{request.FileType.ToString().ToLower()}");

                // Create directory if it does not exist
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                // Write the file to the disk
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }


}