using System;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;

namespace Systems
{
    public class PenguinSystem : MonoBehaviour
    {
        [SerializeField] private GameObject greenPenguinPrefab;
        [SerializeField] private GameObject redPenguinPrefab;
        [SerializeField] private GameObject bluePenguinPrefab;
        private Dictionary<Location, Vector3> _tilePositions;
        private readonly Dictionary<Location, Transform> _penguins = new();

        private void Start()
        {
            _tilePositions = FindObjectsByType<InteractableTile>(FindObjectsSortMode.None)
                .ToDictionary(tile => tile.location, tile => tile.transform.position);
        }

        public void PlacePenguin(Location location, Team team)
        {
            var position = _tilePositions[location];
            var prefab = team switch
            {
                Team.Blue => bluePenguinPrefab,
                Team.Green => greenPenguinPrefab,
                Team.Red => redPenguinPrefab,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, null)
            };
            var penguin = Instantiate(prefab, position, Quaternion.identity);
            _penguins.Add(location, penguin.transform);
        }

        public void MovePenguin(Location from, Location to)
        {
            if (_tilePositions.ContainsKey(from) == false)
                throw new Exception("No such from-location stored in PenguinSystem.");
            
            var penguin = _penguins[from];
            penguin.position = _tilePositions[to];
            _penguins.Remove(from);
            _penguins.Add(to, penguin);
        }
    }
}