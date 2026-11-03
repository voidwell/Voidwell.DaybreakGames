using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/grades")]
public class GradesController : Controller
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpGet]
    [OutputCache(PolicyName = CachePolicies.ReferenceData)]
    [ResponseCache(Duration = CachePolicies.ReferenceDataClientSeconds, Location = ResponseCacheLocation.Any)]
    public ActionResult GetGrades()
    {
        var result = _gradeService.GetAllGrades();
        return Ok(result);
    }
}
