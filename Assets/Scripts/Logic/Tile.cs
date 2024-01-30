using UnityEngine;

namespace Logic
{
    public class Tile
    {
        public readonly Location Location;
        public bool IsOccupied;
        public bool IsWater;
        public Option<Team> Team = Option<Team>.None;
        public readonly int FishCount;
        
        public Tile(Location location, int fishCount)
        {
            Location = location;
            FishCount = fishCount;
        }
    }
}