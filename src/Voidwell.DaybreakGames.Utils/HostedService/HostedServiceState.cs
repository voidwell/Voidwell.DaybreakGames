namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public class HostedServiceState<TService> : HostedServiceState
        where TService : class, IStatefulHostedService
    {
        public HostedServiceState()
        {
            ServiceName = typeof(TService).GetType().Name;
        }

        public void SetServiceName(string serviceName)
        {
            ServiceName = serviceName;
        }
    }

    public abstract class HostedServiceState
    {
        public bool IsRunning { get; internal set; }
        public IStatefulHostedService Service { get; internal set; }
        public string ServiceName { get; protected set; }
    }
}
