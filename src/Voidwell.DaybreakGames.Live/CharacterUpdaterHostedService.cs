using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Live
{
    public class CharacterUpdaterHostedService : StatefulHostedServiceClient
    {
        public CharacterUpdaterHostedService(ICharacterUpdaterService updaterService) : base(updaterService)
        {
        }
    }
}
