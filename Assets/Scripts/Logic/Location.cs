using System;
using UnityEngine;

namespace Logic
{
    public struct Location
    {
        public Location(int q, int r, int s)
        {
            _location = new Vector3Int(q, r, s);
        }

        private Vector3Int _location;
        public int Q => _location.x;
        public int R => _location.y;
        public int S => _location.z;
        public (int, int, int) Coord => (Q, R, S);

        public bool IsSameLine(Location location)
        {
            throw new NotImplementedException();
        }
    }
}