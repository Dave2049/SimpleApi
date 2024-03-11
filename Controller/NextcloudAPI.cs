using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using SimpleApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Assume SmallGroupRequest and other necessary models are defined here

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("")]
    public class NextCloudApiController : ControllerBase
    {
        private readonly ILogger<NextCloudApiController> _logger;

        public NextCloudApiController(ILogger<NextCloudApiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("heartbeat")]
        public IActionResult Heartbeat()
        {
            return Ok(new { status = "ok" });
        }

        [HttpPut("enabled")]
        [Authorize(Policy = "SignCheckPolicy")]
        public async Task<IActionResult> SetEnabled([FromQuery] int enabled)
        {


             var username = User.Identity?.Name; 
            _logger.LogInformation("User {username} is trying to enable the app", username);
            // Your existing logic for handling the enabled state
            // OcsHelper and other utility classes should be accessible here
            // Log with _logger.LogInformation, etc.
            if (enabled == 1)
            {
                   await OcsHelper.SetState(_logger, username);
        await OcsHelper.CallTopMenuApi(_logger);
        await OcsHelper.RegisterFrontendCSS(_logger);
        await OcsHelper.RegisterFrontendJS(_logger); 
                // Enabled logic here
            }
            else if (enabled == 0)
            {
                // Disabled logic here
            }
            else
            {
                // Invalid value logic here
                return BadRequest(new { error = "invalid" });
            }

            return Ok(new { error = "" });
        }

        [HttpPost("hk/{id}")]
        [Authorize(Policy = "SignCheckPolicy")]
        public async Task<IActionResult> HandleHK(int id, [FromBody] SmallGroupRequest request)
        {
            // Assuming this is where your logic for handling the post request starts
            // You might need to adjust based on what exactly OcsHelper.SignCheck and the rest of your processing does
            // Make sure to validate the request and handle it accordingly
            return Ok(); // Or any appropriate response
        }
    }
}
