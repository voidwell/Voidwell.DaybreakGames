namespace Voidwell.DaybreakGames.Utils.StatefulHostedService
{
    public class ServiceState
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
        public object Details { get; set; }
    }
}
