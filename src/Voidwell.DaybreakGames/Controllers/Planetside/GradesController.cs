using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/grades")]
    public class GradesController : Controller
    {
        private readonly IGradeService _gradeService;

        public GradesController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        public IEnumerable<StatGrade> GetGrades()
        {
            return _gradeService.GetAllGrades();
        }
    }
}
