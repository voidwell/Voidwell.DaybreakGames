using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/vehicle")]
public class VehicleController : Controller
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    [OutputCache(PolicyName = CachePolicies.ReferenceData)]
    [ResponseCache(Duration = CachePolicies.ReferenceDataClientSeconds, Location = ResponseCacheLocation.Any)]
    public async Task<ActionResult> GetAllVehiclesAsync()
    {
        var result = await _vehicleService.GetAllVehicles();
        return Ok(result);
    }
}
