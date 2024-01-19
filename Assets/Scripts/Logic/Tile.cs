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
        
        public Tile(Location location)
        {
            Location = location;

            int SpawnFish()
            {
                var percent = Random.value;
                if (percent < .18f) return 3;
                if (percent < .53f) return 2;
                return 1;
            }
            
            FishCount = SpawnFish();
        }
    }
}