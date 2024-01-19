namespace Logic
{
    public class Tile
    {
        public readonly Location Location;
        public bool IsOccupied;
        public bool IsWater;
        public Option<Team> Team = Option<Team>.None;
        
        public Tile(Location location)
        {
            Location = location;
        }
    }
}