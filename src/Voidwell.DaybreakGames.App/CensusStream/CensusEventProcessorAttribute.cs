using System;

namespace Voidwell.DaybreakGames.CensusStream
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CensusEventProcessorAttribute : Attribute
    {
        public string EventName { get; private set; }

        public CensusEventProcessorAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
