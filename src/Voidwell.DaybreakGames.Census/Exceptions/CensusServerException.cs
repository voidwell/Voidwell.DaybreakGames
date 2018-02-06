using System;

namespace Voidwell.DaybreakGames.Census.Exceptions
{
    public class CensusServerException : Exception
    {
        public CensusServerException()
        {
        }

        public CensusServerException(string message) : base(message)
        {
        }
    }
}
