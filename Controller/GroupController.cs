using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleApi.Models;

namespace SimpleApi.Controllers
{
    [ApiController]
    [Route("")]
    public class GroupController(ILogger<GroupController> _logger) : ControllerBase
    {

        [HttpPost("group/{id}")]
        [Authorize(Policy = "SignCheckPolicy")]
        public async Task<IActionResult> createGroup(int id, [FromBody] SmallGroupRequest request)
        {
            // Assuming this is where your logic for handling the post request starts
            // You might need to adjust based on what exactly OcsHelper.SignCheck and the rest of your processing does
            // Make sure to validate the request and handle it accordingly
            return Ok(); // Or any appropriate response
        }


    }
}