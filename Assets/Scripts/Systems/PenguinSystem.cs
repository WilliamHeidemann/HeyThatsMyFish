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
        [SerializeField] private GameObject yellowPenguinPrefab;
        [SerializeField] private Vector3 penguinOffsetVector;
        private Dictionary<Location, Vector3> _tilePositions;
        private Dictionary<Location, Transform> _penguins;

        public void PlacePenguin(Location location, Team team)
        {
            var position = _tilePositions[location];
            var prefab = team switch
            {
                Team.Blue => bluePenguinPrefab,
                Team.Green => greenPenguinPrefab,
                Team.Red => redPenguinPrefab,
                Team.Yellow => yellowPenguinPrefab,
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
            LeanTween.move(penguin.gameObject, _tilePositions[to], 0.5f).setEaseInQuad();
            _penguins.Remove(from);
            _penguins.Add(to, penguin);
        }

        public void InitializePenguinSystem()
        {
            _tilePositions = FindObjectsByType<InteractableTile>(FindObjectsSortMode.None)
                .ToDictionary(tile => tile.location, tile => tile.transform.position + penguinOffsetVector);
            _penguins?.Values.ToList().ForEach(penguinTransform => Destroy(penguinTransform.gameObject));
            _penguins = new Dictionary<Location, Transform>();
        }
    }
}