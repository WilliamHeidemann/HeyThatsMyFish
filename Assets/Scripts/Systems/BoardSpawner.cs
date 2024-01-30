using System;
using System.Collections.Generic;
using Logic;
using Settings;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardSpawner : MonoBehaviour 
{
    [SerializeField] private float padding;
    [SerializeField] private InteractableTile tilePrefab;
    [SerializeField] private Transform boardInstance;
    
    public void SpawnBoard(BoardType boardType, int boardSize)
    {
        DestroyBoard();
        var board = boardType switch
        {
            BoardType.Square => Board.CreateSquareBoard(boardSize),
            BoardType.Hexagon => Board.CreateHexagonBoard(boardSize, 0),
            BoardType.Random => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException()
        };


        foreach (var tile in board.Tiles)
        {
            var position = CalculatePosition(tile.Location.Q, tile.Location.R);
            var interactableTile = Instantiate(tilePrefab, position, Quaternion.identity, boardInstance);
            interactableTile.location = tile.Location;
        }
    }
    
    private Vector3 CalculatePosition(int q, int r)
    {
        var xOffset = (Mathf.Sqrt(3.0f) * (r + 0.5f * q));
        var yOffset = (3.0f / 2.0f) * q;
        const float zOffset = 0f;

        // Apply padding
        xOffset += q * padding;
        yOffset += r * padding;
        // zOffset += s * padding;
        return new Vector3(xOffset, yOffset, zOffset) * 0.7f;
    }

    public void DestroyBoard()
    {
        if (boardInstance == null) return;
        while (boardInstance.childCount > 0)
        {
            DestroyImmediate(boardInstance.GetChild(0).gameObject);
        }
    }
}
