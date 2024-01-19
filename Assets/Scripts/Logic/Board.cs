using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Logic
{
    public class Board
    {
        public readonly IEnumerable<Tile> Tiles;
        public readonly IEnumerable<Location> Locations;

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
                    var location = new Location(q, r, s);
                    tiles.Add(new Tile(location));
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
                    var location = new Location(q, r, s);
                    tiles.Add(new Tile(location));
                }
            }

            return new Board(tiles);
        }

        private Board(IEnumerable<Tile> tiles)
        {
            Tiles = tiles;
            Locations = tiles.Select(tile => tile.Location);
        }

        public IEnumerable<Location> ReachableLocations(Location origin)
        {
            bool TileExists(Location location) => Locations.Any(l => l == location);
            Tile ToTile(Location location) => Tiles.First(tile => tile.Location == location);
            
            var backwardQ = new Location(0, -1, 1);
            var forwardQ = new Location(0, 1, -1);
            var backwardR = new Location(-1, 0, 1);
            var forwardR = new Location(1, 0, -1);
            var backwardS = new Location(1, -1, 0);
            var forwardS = new Location(-1, 1, 0);
            var activeDirections = new List<Location> { backwardQ, forwardQ, backwardR, forwardR, backwardS, forwardS };

            var distance = 1;
            
            while (activeDirections.Any())
            {
                var nextDirections = new List<Location>();
                foreach (var direction in activeDirections)
                {
                    var location = origin + direction * distance;
                    if (!TileExists(location)) continue;
                    var tile = ToTile(location);
                    if (tile.IsOccupied || tile.IsWater) continue;
                    nextDirections.Add(direction);
                    yield return location;
                }

                distance++;
                activeDirections = nextDirections;
            }
        }

        public bool HasMovesLeft(Team team)
        {
            var penguinTiles = Tiles.Where(tile =>
            {
                if (tile.Team.IsSome(out var tileTeam))
                {
                    return tileTeam == team;
                }
                return false;
            });
            
            return penguinTiles.SelectMany(tile => ReachableLocations(tile.Location)).Any();
        }

        public bool HasTile(Location location, out Tile tile)
        {
            tile = Tiles.FirstOrDefault(tile => tile.Location == location);
            return Locations.Contains(location);
        }

        public bool TwoPlayersCanMove()
        {
            var redHasMovesLeft = HasMovesLeft(Team.Red);
            var blueHasMovesLeft = HasMovesLeft(Team.Blue);
            return redHasMovesLeft && blueHasMovesLeft;
        }

        public int RemainingPoints(Team team)
        {
            // just award all fish in the area to the team, even though some might not be able to be caught
            // bfs in 6 directions
            // if a tile is water, dont include it
            var backwardQ = new Location(0, -1, 1);
            var forwardQ = new Location(0, 1, -1);
            var backwardR = new Location(-1, 0, 1);
            var forwardR = new Location(1, 0, -1);
            var backwardS = new Location(1, -1, 0);
            var forwardS = new Location(-1, 1, 0);
            var directions = new List<Location> { backwardQ, forwardQ, backwardR, forwardR, backwardS, forwardS };

            var current = Tiles.Where(tile =>
            {
                if (tile.Team.IsSome(out var tileTeam))
                {
                    return tileTeam == team;
                }
                return false;
            }).ToList();

            var seen = new HashSet<Tile>();
            while (current.Any())
            {
                var next = new List<Tile>();

                current.ForEach(tile =>
                {
                    directions.ForEach(direction =>
                    {
                        var neighborLocation = tile.Location + direction;
                        if (!HasTile(neighborLocation, out var neighborTile)) return;
                        if (seen.Contains(neighborTile) || neighborTile.IsWater || neighborTile.IsOccupied) return;
                        seen.Add(neighborTile);
                        next.Add(neighborTile);
                    });
                });

                current = next;
            }

            return seen.Sum(tile => tile.FishCount);
        }
    }

    public static class PathExtensionMethods
    {
        public static bool IsBlocked(this IEnumerable<Tile> path) => path.Any(tile => tile.IsOccupied || tile.IsWater);
    }
}