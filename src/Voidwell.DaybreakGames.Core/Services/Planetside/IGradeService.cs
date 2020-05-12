using System.Collections.Generic;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IGradeService
    {
        IEnumerable<StatGrade> GetAllGrades();
        string GetGradeByDelta(double? delta);
    }
}
