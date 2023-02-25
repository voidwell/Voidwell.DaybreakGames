using System;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public class HostedServiceState<TService> : HostedServiceState
        where TService : class, IStatefulHostedService
    {
        public HostedServiceState(IServiceProvider sp)
        {
            _service = new Lazy<IStatefulHostedService>(() => (IStatefulHostedService)sp.GetService(typeof(TService)));
            _name = new Lazy<string>(() => Service.GetType().Name);
        }

        public void SetServiceName(string serviceName)
        {
            _name = new Lazy<string>(() => serviceName);
        }
    }

    public abstract class HostedServiceState
    {
        protected Lazy<IStatefulHostedService> _service;
        protected Lazy<string> _name;

        public bool IsRunning { get; internal set; }

        public IStatefulHostedService Service
        {
            get
            {
                return _service.Value;
            }
        }

        public string ServiceName
        {
            get
            {
                return _name.Value;
            }
        }
    }
}
