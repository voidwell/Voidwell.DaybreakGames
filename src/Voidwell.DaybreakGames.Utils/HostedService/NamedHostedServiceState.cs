namespace Voidwell.DaybreakGames.Utils.HostedService;

/// <summary>
/// State for one of several registrations of the same service type, told apart by name.
/// </summary>
public class NamedHostedServiceState : HostedServiceState
{
    public NamedHostedServiceState(string serviceName, Func<IStatefulHostedService> serviceFactory)
    {
        _service = new Lazy<IStatefulHostedService>(serviceFactory);
        _name = new Lazy<string>(() => serviceName);
    }
}
