using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using Systems;
using UnityEngine;

public class  BoardSystem : MonoBehaviour
{
    private Board _board;
    private ColorSystem _colorSystem;
    private PenguinSystem _penguinSystem;
    private readonly TurnManager _teamManager = new();
    private Option<Location> _selectedLocation = Option<Location>.None;
    private int _penguinsToPlace = 8;
    
    private void Awake()
    {
        _colorSystem = FindFirstObjectByType<ColorSystem>();
        _penguinSystem = FindFirstObjectByType<PenguinSystem>();
        _board = Board.CreateHexagonBoard(5);
    }

    private void Start()
    {
        StartTileSelectionPhase();
    }

    private void StartTileSelectionPhase()
    {
        InteractableTile.TilePointerEnter += HighlightTile;
        InteractableTile.TilePointerExit += ResetAllTileColors;
        InteractableTile.TilePointerClick += PlacePenguin;
    }

    private void StartGamePhase()
    {
        InteractableTile.TilePointerEnter -= HighlightTile;
        InteractableTile.TilePointerClick -= PlacePenguin;
        
        InteractableTile.TilePointerEnter += InteractableTileHoverEnter;
        InteractableTile.TilePointerClick += InteractableTileClick;
    }
    
    private void HighlightTile(Location location)
    {
        _colorSystem.ColorTile(location, ColorSystem.ColorType.Clickable);
        print("Coloring");
    }

    private void PlacePenguin(Location location)
    {
        if (!_board.HasTile(location, out var tile)) return;
        if (tile.IsOccupied) return;
        ResetAllTileColors(location);
        tile.IsOccupied = true;
        tile.Team = Option<Team>.Some(_teamManager.TeamToMove());
        _penguinSystem.PlacePenguin(location, _teamManager.TeamToMove());
        _teamManager.NextTeam();

        _penguinsToPlace--;
        if (_penguinsToPlace == 0) StartGamePhase();
    }

    private void InteractableTileHoverEnter(Location location)
    {
        var reachableLocations = _board.ReachableLocations(location);
        _colorSystem.ColorTiles(reachableLocations, ColorSystem.ColorType.Clickable);
        _colorSystem.ColorTile(location, ColorSystem.ColorType.Clickable);
    }

    private void ResetAllTileColors(Location location)
    {
        _colorSystem.ColorTiles(_board.Locations, ColorSystem.ColorType.Default);
    }

    private void InteractableTileClick(Location location)
    {
        _colorSystem.ColorTile(location, ColorSystem.ColorType.Clicked);
    }
}
