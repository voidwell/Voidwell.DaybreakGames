﻿using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class GradeService : IGradeService
    {
        private readonly IEnumerable<StatGrade> _grades = new[] {
                new StatGrade { Grade = "S", Delta = 4.01 },
                new StatGrade { Grade = "M**", Delta = 3.5 },
                new StatGrade { Grade = "M*", Delta = 3.25 },
                new StatGrade { Grade = "M++", Delta = 3 },
                new StatGrade { Grade = "M+", Delta = 2.8 },
                new StatGrade { Grade = "M", Delta = 2.6 },
                new StatGrade { Grade = "A**", Delta = 2.4 },
                new StatGrade { Grade = "A*", Delta = 2 },
                new StatGrade { Grade = "A++", Delta = 1.65 },
                new StatGrade { Grade = "A+", Delta = 1.45 },
                new StatGrade { Grade = "A", Delta = 1.25 },
                new StatGrade { Grade = "B**", Delta = 1 },
                new StatGrade { Grade = "B*", Delta = 0.84 },
                new StatGrade { Grade = "B++", Delta = 0.7 },
                new StatGrade { Grade = "B+", Delta = 0.45 },
                new StatGrade { Grade = "B", Delta = 0.39 },
                new StatGrade { Grade = "C**", Delta = 0.325 },
                new StatGrade { Grade = "C*", Delta = 0.25 },
                new StatGrade { Grade = "C++", Delta = 0.125 },
                new StatGrade { Grade = "C+", Delta = -0.125 },
                new StatGrade { Grade = "C", Delta = -0.25 },
                new StatGrade { Grade = "C", Delta = -0.325 },
                new StatGrade { Grade = "C-", Delta = -0.39 },
                new StatGrade { Grade = "C--", Delta = -0.45 },
                new StatGrade { Grade = "D", Delta = -0.7 },
                new StatGrade { Grade = "D-", Delta = -0.84 },
                new StatGrade { Grade = "D--", Delta = -1 },
                new StatGrade { Grade = "D*", Delta = -1.25 },
                new StatGrade { Grade = "D**", Delta = -1.45 },
                new StatGrade { Grade = "F", Delta = -1.65 },
                new StatGrade { Grade = "F-", Delta = -2 },
                new StatGrade { Grade = "F--", Delta = -2 },
                new StatGrade { Grade = "F*", Delta = -2.6 },
                new StatGrade { Grade = "F**", Delta = -2.8 },
                new StatGrade { Grade = "T", Delta = -3 },
                new StatGrade { Grade = "T-", Delta = -3.25 },
                new StatGrade { Grade = "T*", Delta = -3.5 }
            };

        public IEnumerable<StatGrade> GetAllGrades()
        {
            return _grades;
        }

        public string GetGradeByDelta(double? delta)
        {
            var allGrades = _grades.OrderByDescending(a => a.Delta);

            if (delta == null)
            {
                return allGrades.Last().Grade;
            }

            var grade = allGrades.FirstOrDefault(a => delta >= a.Delta);
            if (grade != null)
            {
                return grade.Grade;
            }

            return allGrades.Last().Grade;
        }
    }
}
