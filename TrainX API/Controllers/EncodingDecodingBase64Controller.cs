using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrainX_API.Helpers;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncodingDecodingBase64Controller : ControllerBase
    {

        [HttpPost("encode")]
        public async Task<IActionResult> EncodeFileToBase64WithMetadata(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please provide a valid file.");
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Copy the file to memory stream
                    await file.CopyToAsync(memoryStream);

                    // Convert the file to a Base64 string
                    string base64Data = Convert.ToBase64String(memoryStream.ToArray());

                    // Add MIME type metadata
                    string mimeType = file.ContentType;
                    string dataUri = $"data:{mimeType};base64,{base64Data}";

                    return Ok(dataUri);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("decode")]
        [Consumes("application/json")]
        public async Task<IActionResult> DecodeFile([FromBody] FileBase64Request request)
        {
            if (string.IsNullOrEmpty(request.Base64Data))
                return BadRequest("Base64 data is required.");

            try
            {
                byte[] fileBytes = Convert.FromBase64String(request.Base64Data);

                var filePath = Path.Combine("Attachments", $"{Guid.NewGuid()}.{request.FileType.ToString().ToLower()}");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

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