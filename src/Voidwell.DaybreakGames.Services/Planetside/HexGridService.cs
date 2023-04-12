using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models.GridMap;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class HexGridService : IHexGridService
    {
        private const double Width = 50.0 / 8.0;
        private const double Hex_B = Width / 2.0;
        private static readonly double Hex_C = Hex_B / Math.Sqrt(3) * 2.0;

        private readonly IMapRepository _mapRepository;

        public HexGridService(IMapRepository mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public async Task<IEnumerable<GridMapRegion>> GetMapForZoneAsync(int zoneId)
        {
            var mapHexs = await _mapRepository.GetMapHexsByZoneIdAsync(zoneId);

            var hexRegionGroups = mapHexs.GroupBy(a => a.MapRegionId);

            var mapRegions = new List<GridMapRegion>();

            foreach (var region in  hexRegionGroups)
            {
                var mapRegion = new GridMapRegion
                {
                    Vertices = CalculateGridVerticesForRegion(region.ToList()),
                };

                mapRegions.Add(mapRegion);
            }

            return mapRegions;
        }

        public IEnumerable<GridMapVertex> CalculateGridVerticesForRegion(IEnumerable<MapHex> hexes)
        {
            var regionLines = hexes.SelectMany(ConvertHexToGridMapLines).ToList();

            var outerLines = CalculateOuterLines(regionLines);

            return CalculateOuterVerts(outerLines);
        }

        private List<GridMapLine> ConvertHexToGridMapLines(MapHex hex)
        {
            var x = (2.0 * hex.XPos + hex.YPos) / 2.0 * Width;

            double y;
            if (hex.YPos % 2 == 1)
            {
                var t = Math.Floor(hex.YPos / 2.0);
                y = (Hex_C * t) + (2.0 * Hex_C * (t + 1.0)) + (Hex_C / 2.0);
            }
            else
            {
                y = (3.0 * Hex_C * hex.YPos) / 2.0 + Hex_C;
            }

            var hexVerts = new GridMapVertex[]
            {
                    new(x - Hex_B, y - (Hex_C / 2.0)),
                    new(x, y - Hex_C),
                    new(x + Hex_B, y - (Hex_C / 2.0)),
                    new(x + Hex_B, y + (Hex_C / 2.0)),
                    new(x, y + Hex_C),
                    new(x - Hex_B, y + (Hex_C / 2.0))
            };

            return new List<GridMapLine>
            {
                new(hexVerts[0], hexVerts[1]),
                new(hexVerts[1], hexVerts[2]),
                new(hexVerts[2], hexVerts[3]),
                new(hexVerts[3], hexVerts[4]),
                new(hexVerts[4], hexVerts[5]),
                new(hexVerts[5], hexVerts[0])
            };
        }

        private List<GridMapLine> CalculateOuterLines(List<GridMapLine> regionLines)
        {
            var outerLines = new List<GridMapLine>();

            for (var i = 0; i < regionLines.Count; i++)
            {
                var flag = false;

                for (var k = 0; k < regionLines.Count; k++)
                {
                    if (i != k && regionLines[i].Equals(regionLines[k]))
                    {
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                {
                    outerLines.Add(regionLines[i]);
                }
            }

            return outerLines;
        }

        private List<GridMapVertex> CalculateOuterVerts(List<GridMapLine> outerLines)
        {
            // Calculate outer verts
            var outerVerts = new List<GridMapVertex> { outerLines[0].Vertex1, outerLines[0].Vertex2 };
            outerLines.RemoveAt(0);

            while (outerLines.Count > 0)
            {
                GridMapVertex vertMatch = null;

                for (var i = 0; i < outerLines.Count; i++)
                {
                    if (outerLines[i].Vertex1.Equals(outerVerts.Last()))
                    {
                        vertMatch = outerLines[i].Vertex2;
                        outerLines.RemoveAt(i);
                        break;
                    }
                }

                if (vertMatch != null)
                {
                    outerVerts.Add(vertMatch);
                }
                else
                {
                    break;
                }
            }

            if (outerVerts.Count > 1 && outerVerts.First().Equals(outerVerts.Last()))
            {
                outerVerts.RemoveAt(outerVerts.Count - 1);
            }

            return outerVerts;
        }
    }
}
