using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/world")]
    public class WorldController : Controller
    {
        private readonly IWorldService _worldService;

        public WorldController(IWorldService worldService)
        {
            _worldService = worldService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllWorlds ()
        {
            var result = await _worldService.GetAllWorlds();
            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<ActionResult> GetWorldActivity([FromQuery(Name = "worldId")]int? worldId, [FromQuery(Name = "period")]int? hours)
        {
            if (!worldId.HasValue)
            {
                return BadRequest("Missing required 'worldId' query parameter");
            }

            if (!hours.HasValue || hours > 24)
            {
                return BadRequest("Missing or invalid required 'period' query parameter.");
            }

            var result = await _worldService.GetWorldActivity(worldId.Value, hours.Value);

            return Ok(result);
        }

        [HttpGet("population")]
        public async Task<ActionResult> GetWorldPopulationHistory([FromQuery(Name = "q")]string sWorldIds)
        {
            var worldIds = sWorldIds.Split(",").Select(a => int.Parse(a));
            if (!worldIds.Any())
            {
                return BadRequest("Must select at least one server");
            }

            var result = await _worldService.GetWorldPopulationHistory(worldIds, DateTime.MinValue, DateTime.UtcNow.Date.AddSeconds(-1));

            var paddedStats = PadPeriods(result);

            return Ok(paddedStats);
        }

        private static Dictionary<int, IEnumerable<DailyPopulation>> PadPeriods(Dictionary<int, IEnumerable<DailyPopulation>> stats)
        {
            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;

            foreach (var stat in stats)
            {
                if (!stat.Value.Any())
                {
                    continue;
                }

                var minStatDate = stat.Value.Min(a => a.Date);
                if (minStatDate < minDate)
                {
                    minDate = minStatDate;
                }

                var maxStatDate = stat.Value.Max(a => a.Date);
                if (maxStatDate > maxDate)
                {
                    maxDate = maxStatDate;
                }
            }

            var emptyDates = new List<DailyPopulation>();
            for (var d = 0; d < (maxDate - minDate).Days; d++)
            {
                var emptyStat = new DailyPopulation { Date = minDate.AddDays(d) };
                emptyDates.Add(emptyStat);
            }

            var paddedStats = new Dictionary<int, IEnumerable<DailyPopulation>>();
            foreach (var stat in stats)
            {
                paddedStats[stat.Key] = stat.Value.Union(emptyDates, new DailyPopulationComparer()).OrderBy(a => a.Date);
            }

            return paddedStats;
        }
    }
}
