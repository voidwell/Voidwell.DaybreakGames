namespace Voidwell.DaybreakGames.Utils.HostedService;

public interface IStatefulHostedServiceManager
{
    Task<IEnumerable<ServiceState>> GetServiceStatusAsync(CancellationToken cancellationToken);
    Task<ServiceState?> GetServiceStatusAsync(string serviceName, CancellationToken cancellationToken);
    Task StartServiceAsync(string serviceName, CancellationToken cancellationToken);
    Task StopServiceAsync(string serviceName, CancellationToken cancellationToken);
    bool VerifyServiceExists(string serviceName);
}
