using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers;

[Route("psb")]
[Authorize(Roles = AuthConstants.Roles.AdministratorOrPsb)]
public class PSBUtilityController : Controller
{
    private readonly IPSBUtilityService _psbUtilityService;

    public PSBUtilityController(IPSBUtilityService psbUtilityService)
    {
        _psbUtilityService = psbUtilityService;
    }

    [HttpGet("sessions")]
    public async Task<ActionResult> GetLastOnlinePSBAccountsAsync()
    {
        var results = await _psbUtilityService.GetLastOnlinePSBAccounts();
        return Ok(results);
    }
}
