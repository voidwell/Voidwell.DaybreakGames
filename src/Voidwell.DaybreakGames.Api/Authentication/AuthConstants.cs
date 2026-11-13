namespace Voidwell.DaybreakGames.Api.Authentication;

public static class AuthConstants
{
    public static class Policies
    {
        public const string Mutterblack = "Mutterblack";
        public const string PlanetsideBattlesOrganizer = "PlanetsideBattlesOrganizer";
    }

    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string PlanetsideBattlesOrganizer = "PSB";
    }

    /// <summary>
    /// Token claim names. Inbound claim mapping is off, so claims keep the names the token gives them.
    /// </summary>
    public static class Claims
    {
        public const string Subject = "sub";
        public const string ClientId = "client_id";
    }
}
