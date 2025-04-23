using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralSerializationController : ControllerBase
    {
        [HttpPost("serialize-object")]
        public IActionResult SerializeObject([FromBody] object data)
        {
            string jsonString = JsonSerializer.Serialize(data);  // Convert object to JSON string
            return Ok(jsonString);  // Return the JSON string
        }

        // Endpoint to deserialize a JSON string back to object
        [HttpPost("deserialize-json")]
        public IActionResult DeserializeJson([FromBody] string jsonString)
        {
            object deserializedObject = JsonSerializer.Deserialize<object>(jsonString);  // Deserialize JSON string to object
            return Ok(deserializedObject);  // Return the deserialized object
        }

        // Endpoint to deserialize a JSON string to dynamic type
        [HttpPost("deserialize-dynamic")]
        public IActionResult DeserializeDynamic([FromBody] string jsonString)
        {
            dynamic deserializedObject = JsonSerializer.Deserialize<dynamic>(jsonString);  // Deserialize JSON to dynamic object
            return Ok(deserializedObject);  // Return the dynamic object
        }
    }
}
