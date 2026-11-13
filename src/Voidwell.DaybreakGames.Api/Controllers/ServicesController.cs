using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.Common.Configuration;
using Voidwell.Common.State;
using Voidwell.DaybreakGames.Api.Authentication;

namespace Voidwell.DaybreakGames.Api.Controllers;

[Route("ps2/services")]
[Authorize(Roles = AuthConstants.Roles.Administrator)]
public class ServicesController : Controller
{
    private readonly IStatefulHostedServiceManager _serviceManager;

    public ServicesController(IStatefulHostedServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetAllServiceStatusAsync()
    {
        var states = await _serviceManager.GetServiceStatusAsync(CancellationToken.None);

        return Ok(states);
    }

    [HttpGet("{serviceName}/status")]
    public async Task<ActionResult> GetServiceStatusAsync(string serviceName)
    {
        var state = await _serviceManager.GetServiceStatusAsync(serviceName, CancellationToken.None);
        if (state == null)
        {
            return NotFound();
        }

        return Ok(state);
    }

    [HttpPost("{serviceName}/enable")]
    public Task<ActionResult> PostEnableServiceAsync(string serviceName)
    {
        return ChangeServiceStateAsync(serviceName, _serviceManager.StartServiceAsync);
    }

    [HttpPost("{serviceName}/disable")]
    public Task<ActionResult> PostDisableServiceAsync(string serviceName)
    {
        return ChangeServiceStateAsync(serviceName, _serviceManager.StopServiceAsync);
    }

    private async Task<ActionResult> ChangeServiceStateAsync(string serviceName,
        Func<string, string?, CancellationToken, Task> changeState)
    {
        if (!_serviceManager.VerifyServiceExists(serviceName))
        {
            return NotFound();
        }

        try
        {
            await changeState(serviceName, User.FindFirst(AuthConstants.Claims.Subject)?.Value, CancellationToken.None);
        }
        catch (ApplicationConfigurationLockedException ex)
        {
            // Appsettings or an environment variable sets this service's state, so it can't be changed at runtime
            return Conflict(new { message = ex.Message, lockedBy = ex.LockedBy });
        }

        var status = await _serviceManager.GetServiceStatusAsync(serviceName, CancellationToken.None);

        return Ok(status);
    }
}
