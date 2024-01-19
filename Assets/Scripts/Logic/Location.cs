using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Logic
{
    [Serializable]
    public struct Location
    {
        public override bool Equals(object obj)
        {
            return obj is Location other && Equals(other);
        }

        public override int GetHashCode()
        {
            return internalLocation.GetHashCode();
        }

        public Location(int q, int r, int s)
        {
            internalLocation = new Vector3Int(q, r, s);
        }

        [SerializeField] private Vector3Int internalLocation;
        public int Q => internalLocation.x;
        public int R => internalLocation.y;
        public int S => internalLocation.z;
        public (int, int, int) Coord => (Q, R, S);
        public override string ToString() => $"{Q}, {R}, {S}";

        public static Location operator *(Location location, int number) => new(location.Q * number, location.R * number, location.S * number);
        public static Location operator *(int number, Location location) => location * number;
        public static Location operator +(Location location1, Location location2) =>
            new Location(location1.Q + location2.Q, location1.R + location2.R, location1.S + location2.S);
        public static bool operator ==(Location location1, Location location2) => location1.Coord == location2.Coord;
        public static bool operator !=(Location location1, Location location2) => !(location1 == location2);
        public bool Equals(Location other)
        {
            return internalLocation.Equals(other.internalLocation);
        }
        public bool IsSameLine(Location location) => location.Q == Q || location.R == R || location.S == S;
    }
}