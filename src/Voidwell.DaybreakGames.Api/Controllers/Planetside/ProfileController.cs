using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/profile")]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileservice)
    {
        _profileService = profileservice;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllProfilesAsync()
    {
        var result = await _profileService.GetAllProfiles();
        return Ok(result);
    }
}
