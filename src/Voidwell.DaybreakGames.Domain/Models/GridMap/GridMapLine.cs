using System;

namespace Voidwell.DaybreakGames.Domain.Models.GridMap
{
    public class GridMapLine
    {
        public GridMapLine(GridMapVertex vertex1, GridMapVertex vertex2)
        {
            Vertex1 = vertex1;
            Vertex2 = vertex2;
        }

        public GridMapVertex Vertex1 { get; set; }
        public GridMapVertex Vertex2 { get; set; }

        public bool ContainsVertex(GridMapVertex vertex)
        {
            return Vertex1.Equals(vertex) || Vertex2.Equals(vertex);
        }

        public override bool Equals(object obj)
        {
            if (obj is GridMapLine other)
            {
                return (Vertex1.Equals(other.Vertex1) && Vertex2.Equals(other.Vertex2)) || (Vertex1.Equals(other.Vertex2) && Vertex2.Equals(other.Vertex1));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Vertex1, Vertex2);
        }
    }
}
