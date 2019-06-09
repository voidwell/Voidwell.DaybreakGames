using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.HostedServices
{
    public class CharacterUpdaterHostedService : StatefulHostedServiceClient
    {
        public CharacterUpdaterHostedService(ICharacterUpdaterService updaterService) : base(updaterService)
        {
        }
    }
}
