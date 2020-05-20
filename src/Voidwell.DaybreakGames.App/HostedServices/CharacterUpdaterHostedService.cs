using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.App.HostedServices
{
    public class CharacterUpdaterHostedService : StatefulHostedServiceClient
    {
        public CharacterUpdaterHostedService(ICharacterUpdaterService updaterService) : base(updaterService)
        {
        }
    }
}
