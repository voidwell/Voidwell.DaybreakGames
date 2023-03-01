﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface IObjectiveRepository
    {
        Task<IEnumerable<Objective>> GetObjectivesByGroupIdAsync(IEnumerable<int> groupIds);
    }
}