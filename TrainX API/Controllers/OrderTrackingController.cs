using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TrainX_API.Helpers;

namespace TrainX_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTrackingController : ControllerBase
    {
        private readonly IHubContext<TrackingHub> _hubContext;

        public OrderTrackingController(IHubContext<TrackingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// HTTP endpoint to push a location update for an order.
        /// Flutter can call this if you prefer REST over Socket for sending coords.
        /// </summary>
        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateDto dto)
        {
            // Optionally validate dto here (order exists, bounds checks, etc.)

            await _hubContext
                  .Clients
                  .Group(dto.OrderId)
                  .SendAsync("ReceiveLocationUpdate",
                             dto.OrderId,
                             dto.Latitude,
                             dto.Longitude,
                             DateTime.UtcNow);

            return Ok(new { status = "sent" });
        }

        /// <summary>
        /// HTTP endpoint to add a connection to an order's group.
        /// Flutter calls this once on connect.
        /// </summary>
        [HttpPost("join-order")]
        public async Task<IActionResult> JoinOrder([FromBody] OrderGroupDto dto)
        {
            // You cannot add via HubContext to group a specific connection;
            // instead, instruct client to call the Hub method JoinOrderGroup.
            return Ok(new { message = "Please invoke SignalR hub method 'JoinOrderGroup' with orderId." });
        }
    }
}
