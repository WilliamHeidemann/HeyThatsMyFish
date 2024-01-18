using System;
using System.Collections.Generic;

namespace Logic
{
    public class Board
    {
        public readonly IEnumerable<Tile> Tiles;

        public static Board CreateHexagonBoard(int size)
        {
            var tiles = new List<Tile>();
            for (var q = -size + 1; q < size; q++)
            {
                var r1 = Math.Max(-size + 1, -q - size + 1);
                var r2 = Math.Min(size - 1, -q + size - 1);
                for (var r = r1; r <= r2; r++)
                {
                    var s = -q - r;
                    tiles.Add(new Tile(new Location(q, r, s)));
                }
            }

            return new Board(tiles);
        }

        public static Board CreateSquareBoard(int size)
        {
            var tiles = new List<Tile>();
            for (int q = 0; q < size; q++)
            {
                int colOffset = q >> 1; // integer division by 2
                for (int r = -colOffset; r < size - colOffset; r++)
                {
                    int s = -q - r;
                    tiles.Add(new Tile(new Location(q, r, s)));
                }
            }

            return new Board(tiles);
        }

        private Board(IEnumerable<Tile> tiles)
        {
            Tiles = tiles;
        }
        
        public List<Tile> GetPath(Location from, Location to)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tile> ReachableTiles(Location origin)
        {
            throw new NotImplementedException();
        }
    }

    public static class PathExtensionMethods
    {
        public static bool IsBlocked(this IEnumerable<Tile> path)
        {
            throw new NotImplementedException();
        }
    }
}