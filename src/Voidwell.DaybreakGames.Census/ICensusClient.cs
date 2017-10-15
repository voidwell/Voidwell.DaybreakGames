namespace Voidwell.DaybreakGames.Census
{
    public interface ICensusClient
    {
        CensusQuery CreateQuery(string serviceName);
    }
}
