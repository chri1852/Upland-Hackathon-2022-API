using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UplandHackathon2022.API.Contracts.Messages;

namespace UplandHackathon2022.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        public HealthCheckController() { }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<GenericResponse> HealthCheck()
        {
            return Ok(new GenericResponse { Message = "Success" });
        }
    }
}
