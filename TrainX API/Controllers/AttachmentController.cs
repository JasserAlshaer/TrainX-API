using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        [HttpGet("metadata/{filename}")]
        public IActionResult GetFileMetadata(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileInfo = new FileInfo(filePath);

            // Return file metadata (size, name, content type)
            var fileMetadata = new
            {
                fileName = fileInfo.Name,
                fileSize = fileInfo.Length,  // In bytes
                fileExtension = fileInfo.Extension,
                contentType = GetContentType(filePath)  // Mime Type (e.g., image/png, application/pdf, etc.)
            };

            return Ok(fileMetadata);
        }

        [HttpGet("download/{filename}")]
        public IActionResult DownloadFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);
            return File(fileBytes, "application/octet-stream", fileName);
        }
        [HttpGet("list")]
        public IActionResult GetAllFiles()
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");

            if (!Directory.Exists(folderPath))
            {
                return NotFound("No files found.");
            }

            var files = Directory.GetFiles(folderPath)
                                 .Select(f => new FileInfo(f))
                                 .Select(f => new
                                 {
                                     fileName = f.Name,
                                     fileSize = f.Length,  // In bytes
                                     fileExtension = f.Extension
                                 })
                                 .ToList();

            return Ok(files);
        }
        [HttpGet("exists/{filename}")]
        public IActionResult FileExists(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", filename);

            if (System.IO.File.Exists(filePath))
            {
                return Ok("File exists.");
            }

            return NotFound("File not found.");
        }
        [HttpPost("upload")]
        public async Task<string> UploadImageAndGetURL(IFormFile file)
        {
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "attachments");
            if (file == null || file.Length == 0)
            {
                throw new Exception("Please Enter Valid File");
            }
            string newFileURL = DateTime.Now.ToString() + "" + file.FileName;
            string newFileURL2 = Guid.NewGuid().ToString() + "" + file.FileName;
            using (var inputFile = new FileStream(Path.Combine(uploadFolder, newFileURL2), FileMode.Create))
            {
                await file.CopyToAsync(inputFile);
            }
            return "/attachments" + newFileURL2;
        }
        [HttpDelete("delete/{filename}")]
        public IActionResult DeleteFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            System.IO.File.Delete(filePath);
            return Ok($"File {filename} deleted successfully.");
        }
        private string GetContentType(string filePath)
        {
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

            // Map common file extensions to content types
            return fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}
