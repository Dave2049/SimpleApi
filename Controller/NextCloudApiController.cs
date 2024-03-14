using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SimpleApi.Service; // Assume SmallGroupRequest and other necessary models are defined here

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("")]
    public class NextCloudApiController(ILogger<NextCloudApiController> _logger, OcsService ocsService) : ControllerBase
    {
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
                   await ocsService.SetState();
        await ocsService.CallTopMenuApi();
        await ocsService.RegisterFrontendCSS();
        await ocsService.RegisterFrontendJS(); 
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

    }
}
