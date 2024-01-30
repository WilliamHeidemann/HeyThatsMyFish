using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Logic
{
    public class Board
    {
        public readonly IEnumerable<Tile> Tiles;
        public readonly IEnumerable<Location> Locations;

        private static IEnumerable<int> FishToDistribute(int boardSideLength, int penguinsToBePlaced)
        {
            int FishCount()
            {
                if (penguinsToBePlaced > 0)
                {
                    penguinsToBePlaced--;
                    return 1;
                }
                var percent = Random.value;
                if (percent < .18f) return 3;
                if (percent < .53f) return 2;
                return 1;
            }

            var boardTileCount = (int)Mathf.Pow(boardSideLength, 2) * 3 - boardSideLength * 3 + 1;
            var fish = new List<int>();
            Enumerable.Range(0, boardTileCount).ToList().ForEach(_ => fish.Add(FishCount()));
            return fish.OrderBy(_ => Random.value);
        }

        public static Board CreateHexagonBoard(int size, int penguinsToBePlaced)
        {
            var fishData = FishToDistribute(size, penguinsToBePlaced);
            var fish = new Stack<int>();
            foreach (var f in fishData) fish.Push(f);
            
            var tiles = new List<Tile>();
            for (var q = -size + 1; q < size; q++)
            {
                var r1 = Math.Max(-size + 1, -q - size + 1);
                var r2 = Math.Min(size - 1, -q + size - 1);
                for (var r = r1; r <= r2; r++)
                {
                    var s = -q - r;
                    var location = new Location(q, r, s);
                    tiles.Add(new Tile(location, fish.Pop()));
                }
            }

            return new Board(tiles);
        }

        public static Board CreateSquareBoard(int size)
        {
            throw new NotImplementedException();
        }
        
        public static Board CreateRandomBoard(int gameSettingsBoardSize)
        {
            throw new NotImplementedException();
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

        public bool HasMovesLeft(Team team) =>
            Tiles.Where(tile => 
            {
                if (tile.Team.IsSome(out var tileTeam)) return tileTeam == team;
                return false;
            }).SelectMany(tile => ReachableLocations(tile.Location)).Any();

        public bool HasTile(Location location, out Tile tile)
        {
            tile = Tiles.FirstOrDefault(tile => tile.Location == location);
            return Locations.Contains(location);
        }

        public bool TwoPlayersCanMove()
        {
            var playersThatCanMove = Enumerable.Range(0, 4).Count(i => HasMovesLeft((Team)i));
            return playersThatCanMove >= 2;
        }

        public int RemainingPoints(Team team)
        {
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