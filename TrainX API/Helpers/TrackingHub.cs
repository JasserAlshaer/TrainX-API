using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace TrainX_API.Helpers
{
    public class TrackingHub : Hub
    {
        // Client can call this method to send its current location
        public async Task SendLocationUpdate(string orderId, double latitude, double longitude)
        {
            // Broadcast to all clients (or you can filter by group/order)
            await Clients.Group(orderId)
                         .SendAsync("ReceiveLocationUpdate", orderId, latitude, longitude, DateTime.UtcNow);
        }

        // Join a group so only relevant clients get updates for a given order
        public Task JoinOrderGroup(string orderId)
            => Groups.AddToGroupAsync(Context.ConnectionId, orderId);

        public Task LeaveOrderGroup(string orderId)
            => Groups.RemoveFromGroupAsync(Context.ConnectionId, orderId);
    }
    public class LocationUpdateDto
    {
        public string OrderId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class OrderGroupDto
    {
        public string OrderId { get; set; }
    }
    public class DecryptRequest
    {
        public string EncryptedText { get; set; }
        public string Key { get; set; }
    }

    public class AesEncryptionService
    {
        // Method to Encrypt data with Key and IV returned
        public (string EncryptedData, string Key) Encrypt(string plainText)
        {
            // Generate a new AES key and IV for each encryption
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    // Return both the encrypted data and the key used for encryption
                    string encryptedData = Convert.ToBase64String(msEncrypt.ToArray());
                    string key = Convert.ToBase64String(aesAlg.Key);

                    return (encryptedData, key);  // Return tuple containing both encrypted data and key
                }
            }
        }

        // Method to Decrypt data using a provided key and IV
        public string Decrypt(string encryptedText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);  // Get the key from the base64 string

                // We need the IV to decrypt, so we'll generate a random IV for this example
                aesAlg.GenerateIV();  // Just using random IV for this example

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
    public class FileBase64Request
    {
        public string Base64Data { get; set; }
        public FileType FileType { get; set; }
    }
    public enum FileType
    {
        Image,
        Document,
        Audio,
        Video
    }
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            foreach (var parameter in fileParameters)
            {
                operation.Parameters.Remove(operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name));
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties =
                            {
                                [parameter.Name] = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            }
                        }
                    }
                }
                };
            }
        }
    }
}
