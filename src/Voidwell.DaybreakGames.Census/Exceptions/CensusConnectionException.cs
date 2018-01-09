using System;

namespace Voidwell.DaybreakGames.Census.Exceptions
{
    public class CensusConnectionException : Exception
    {
        public CensusConnectionException()
        {
        }

        public CensusConnectionException(string message) : base(message)
        {
        }
    }
}
