using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HashingController : ControllerBase
    {
        [HttpPost("sha1")]
        public IActionResult Sha1([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { Message = "Input data cannot be null or empty." });
            }

            using (var sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return Ok(new { Hash = hashHex });
            }
        }
        [HttpPost("sha224")]
        public IActionResult Sha224([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { Message = "Input data cannot be null or empty." });
            }

            using (var sha256 = SHA256.Create())
            {
                // Compute hash and truncate to 224 bits (28 bytes).
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashHex = BitConverter.ToString(hash.Take(28).ToArray()).Replace("-", "").ToLower();
                return Ok(new { Hash = hashHex });
            }
        }

        [HttpPost("sha256")]
        public IActionResult Sha256([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { Message = "Input data cannot be null or empty." });
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return Ok(new { Hash = hashHex });
            }
        }
        [HttpPost("sha384")]
        public IActionResult Sha384([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { Message = "Input data cannot be null or empty." });
            }

            using (var sha384 = SHA384.Create())
            {
                byte[] hash = sha384.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return Ok(new { Hash = hashHex });
            }
        }
        [HttpPost("sha512")]
        public IActionResult Sha512([FromBody] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { Message = "Input data cannot be null or empty." });
            }

            using (var sha512 = SHA512.Create())
            {
                byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(data));
                string hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return Ok(new { Hash = hashHex });
            }
        }
        [HttpPost("md5")]
        public IActionResult Md5([FromQuery] string data)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Ok(BitConverter.ToString(hash).Replace("-", "").ToLower());
            }
        }
    }
}
