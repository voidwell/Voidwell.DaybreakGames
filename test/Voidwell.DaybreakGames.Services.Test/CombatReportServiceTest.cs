using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class CombatReportServiceTest
{
    private static CombatReportParticipantStats Player(long id, int? factionId)
    {
        var player = new CombatReportParticipantStats(id);
        player.Character!.FactionId = factionId;
        return player;
    }

    private static Death Kill(long attacker, long victim) => new() { AttackerCharacterId = attacker, CharacterId = victim };

    [Fact]
    public void CountFactionKills_GroupsByAttackerAndVictimFaction()
    {
        var players = new Dictionary<long, CombatReportParticipantStats>
        {
            [1] = Player(1, 1), [2] = Player(2, 2), [3] = Player(3, 3), [4] = Player(4, 1)
        };
        var deaths = new[] { Kill(1, 2), Kill(1, 2), Kill(4, 3), Kill(2, 1), Kill(1, 4) };

        var result = CombatReportService.CountFactionKills(deaths, players);

        Assert.Equal(new[] { (1, 1, 1), (1, 2, 2), (1, 3, 1), (2, 1, 1) },
            result.Select(r => (r.AttackerFactionId, r.VictimFactionId, r.Kills)));
    }

    [Fact]
    public void CountFactionKills_LeavesOutSuicidesAndUnknownFactions()
    {
        var players = new Dictionary<long, CombatReportParticipantStats>
        {
            [1] = Player(1, 1), [2] = Player(2, null), [3] = Player(3, 0)
        };
        var deaths = new[] { Kill(1, 1), Kill(0, 1), Kill(1, 2), Kill(3, 1), Kill(1, 99) };

        Assert.Empty(CombatReportService.CountFactionKills(deaths, players));
    }
}
