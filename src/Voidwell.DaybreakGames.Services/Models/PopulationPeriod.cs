using System;

namespace Voidwell.DaybreakGames.Services.Models
{
    public class PopulationPeriod
    {
        public PopulationPeriod() { }
        public PopulationPeriod(DateTime timestamp, int vs, int nc, int tr, int ns)
        {
            Timestamp = timestamp;
            VS = vs;
            NC = nc;
            TR = tr;
            NS = ns;
        }
        public PopulationPeriod(int vs, int nc, int tr, int ns)
        {
            VS = vs;
            NC = nc;
            TR = tr;
            NS = ns;
        }

        public DateTime? Timestamp { get; set; }
        public int VS { get; set; }
        public int NC { get; set; }
        public int TR { get; set; }
        public int NS { get; set; }
    }
}
