using System;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems
{
    public class BoardSpawner : MonoBehaviour 
    {
        [SerializeField] private BoardType boardType;
        [SerializeField] private int boardSize;
        [SerializeField] private float padding;
        [SerializeField] private InteractableTile tilePrefab;
        [SerializeField] private Transform boardInstance;
        [SerializeField] private BoardSystem boardSystem;
        
        public void SpawnBoard()
        {
            DestroyBoard();
            var board = boardType switch
            {
                BoardType.Square => Board.CreateSquareBoard(boardSize),
                BoardType.Hexagonal => Board.CreateHexagonBoard(boardSize),
                _ => throw new ArgumentOutOfRangeException()
            };


            foreach (var tile in board.Tiles)
            {
                var position = CalculatePosition(tile.Location.Q, tile.Location.R);
                var interactableTile = Instantiate(tilePrefab, position, Quaternion.identity, boardInstance);
                interactableTile.boardSystem = boardSystem;
                interactableTile.Location = tile.Location;
            }
            
            boardSystem.Board = board;
        }
        
        private Vector3 CalculatePosition(int q, int r)
        {
            var xOffset = (3.0f / 2.0f) * q;
            var yOffset = (Mathf.Sqrt(3.0f) * (r + 0.5f * q));
            const float zOffset = 0f;

            // Apply padding
            xOffset += q * padding;
            yOffset += r * padding;
            // zOffset += s * padding;

            return new Vector3(xOffset, yOffset, zOffset);
        }

        public void DestroyBoard()
        {
            if (boardInstance == null) return;
            while (boardInstance.childCount > 0)
            {
                DestroyImmediate(boardInstance.GetChild(0).gameObject);
            }
        }
        
        public enum BoardType
        {
            Square,
            Hexagonal
        }
    }
}