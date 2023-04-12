using System.Collections.Generic;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface IGradeService
    {
        IEnumerable<StatGrade> GetAllGrades();
        string GetGradeByDelta(double? delta);
    }
}
