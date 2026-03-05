using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TallahasseePRs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]    
    public sealed class JudgeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            return Ok();
        }
    }
}
