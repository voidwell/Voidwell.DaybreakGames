namespace Voidwell.DaybreakGames.Models
{
    public class ZonePopulation
    {
        public ZonePopulation() { }
        public ZonePopulation(int vs, int nc, int tr)
        {
            VS = vs;
            NC = nc;
            TR = tr;
        }

        public int VS { get; set; }
        public int NC { get; set; }
        public int TR { get; set; }
    }
}
