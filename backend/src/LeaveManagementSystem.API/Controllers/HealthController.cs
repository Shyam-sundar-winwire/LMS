using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "LeaveManagementSystem.API",
            timestampUtc = DateTime.UtcNow
        });
    }
}
