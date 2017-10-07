using System;

namespace Voidwell.DaybreakGames.Census
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UriQueryPropertyAttribute : Attribute
    {
        private string _name;

        public UriQueryPropertyAttribute()
        {
        }

        public UriQueryPropertyAttribute(string name)
        {
            _name = name;
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
