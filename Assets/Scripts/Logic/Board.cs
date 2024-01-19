﻿using System;
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
        
        public List<Tile> GetPath(Location fromExcluded, Location toIncluded)
        {
            throw new NotImplementedException();
            
            var path = new List<Tile>();
            if (fromExcluded.IsSameLine(toIncluded) == false)
            {
                throw new Exception($"Locations are not in the same line. Source: {fromExcluded}. Destination: {toIncluded}.");
            } 
            
            if (fromExcluded.Q == toIncluded.Q)
            {
                
            }
            else if (fromExcluded.R == toIncluded.R)
            {
                
            }
            else if (fromExcluded.S == toIncluded.S)
            {
                
            }



            return path;
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

        public void PlacePenguin(Location location, Team team)
        {
            var tile = Tiles.First(tile => tile.Location == location);
            tile.IsOccupied = true;
            tile.Team = Option<Team>.Some(team);
        }

        public bool HasTile(Location location, out Tile tile)
        {
            tile = Tiles.FirstOrDefault(tile => tile.Location == location);
            return Locations.Contains(location);
        }
    }

    public static class PathExtensionMethods
    {
        public static bool IsBlocked(this IEnumerable<Tile> path) => path.Any(tile => tile.IsOccupied || tile.IsWater);
    }
}