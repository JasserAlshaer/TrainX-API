using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TrainX_API.Helpers;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {

        private readonly AesEncryptionService _aesEncryptionService = new AesEncryptionService();


        [HttpPost("aes/encrypt")]
        public IActionResult AesEncrypt([FromBody] string plainText)
        {
            var result = _aesEncryptionService.Encrypt(plainText);
            return Ok(new
            {
                EncryptedText = result.EncryptedData,
                Key = result.Key
            });
        }

        // Endpoint for AES decryption (Requires both encrypted data and the key)
        [HttpPost("aes/decrypt")]
        public IActionResult AesDecrypt([FromBody] DecryptRequest request)
        {
            string decryptedText = _aesEncryptionService.Decrypt(request.EncryptedText, request.Key);
            return Ok(new { DecryptedText = decryptedText });
        }
    }

}
