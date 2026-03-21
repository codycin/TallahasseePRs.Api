using Microsoft.AspNetCore.Mvc;

namespace TallahasseePRs.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "ok", message = "Backend connected" });
        }
    }
}