using System;

namespace Voidwell.DaybreakGames.Domain.Models.GridMap
{
    public class GridMapVertex
    {
        public GridMapVertex() { }

        public GridMapVertex(double x, double y)
        {
            X = Math.Round(y * 1000.0) / 1000.0;
            Y = Math.Round(x * 1000.0) / 1000.0;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GridMapVertex other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
