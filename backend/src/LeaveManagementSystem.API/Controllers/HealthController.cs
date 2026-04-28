using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var healthReport = await _healthCheckService.CheckHealthAsync();
            
            return Ok(new
            {
                status = healthReport.Status == HealthStatus.Healthy ? "ok" : "degraded",
                service = "LeaveManagementSystem.API",
                version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0",
                timestampUtc = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                checks = healthReport.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.TotalMilliseconds,
                    data = entry.Value.Data
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(503, new
            {
                status = "unhealthy",
                service = "LeaveManagementSystem.API",
                timestampUtc = DateTime.UtcNow,
                error = "Health check service unavailable"
            });
        }
    }

    [HttpGet("ready")]
    public IActionResult Ready()
    {
        return Ok(new { status = "ready", timestampUtc = DateTime.UtcNow });
    }

    [HttpGet("live")]
    public IActionResult Live()
    {
        return Ok(new { status = "alive", timestampUtc = DateTime.UtcNow });
    }
}
