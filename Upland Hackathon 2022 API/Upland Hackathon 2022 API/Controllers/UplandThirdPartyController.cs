using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UplandHackathon2022.API.Contracts.Messages;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages;
using UplandHackaton2022.Api.Abstractions;

namespace Upland_Hackathon_2022_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UplandThirdPartyController : ControllerBase
    {
        private readonly IUplandThirdPartyApiRepository _uplandThirdPartyApiRepository;
        public UplandThirdPartyController(IUplandThirdPartyApiRepository thirdPartyRepo) 
        {
            _uplandThirdPartyApiRepository = thirdPartyRepo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PostUserAppAuthResponse>> Get()
        {
            try
            {
                return Ok(await _uplandThirdPartyApiRepository.PostOTP());
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}