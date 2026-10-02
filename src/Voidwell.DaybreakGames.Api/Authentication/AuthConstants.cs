namespace Voidwell.DaybreakGames.Api.Authentication
{
    public static class AuthConstants
    {
        public static class Policies
        {
            public const string Mutterblack = "Mutterblack";
        }

        public static class Roles
        {
            public const string Administrator = "Administrator";
            public const string Psb = "PSB";
            public const string AdministratorOrPsb = Administrator + "," + Psb;
        }
    }
}
