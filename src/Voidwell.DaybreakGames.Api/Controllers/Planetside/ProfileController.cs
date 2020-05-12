﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/profile")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileservice)
        {
            _profileService = profileservice;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProfiles()
        {
            var result = await _profileService.GetAllProfiles();
            return Ok(result);
        }
    }
}
