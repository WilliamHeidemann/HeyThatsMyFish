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
    private PointSystem _pointSystem;
    private readonly TurnManager _teamManager = new();
    private Option<Location> _selectedLocation = Option<Location>.None;
    private int _penguinsToPlace = 4;
    
    private void Awake()
    {
        _colorSystem = FindFirstObjectByType<ColorSystem>();
        _penguinSystem = FindFirstObjectByType<PenguinSystem>();
        _pointSystem = FindFirstObjectByType<PointSystem>();
        _board = Board.CreateHexagonBoard(5);
        _pointSystem.SetFishSprites(_board.Tiles);
    }

    private void Start()
    {
        StartTileSelectionPhase();
    }

    private void StartTileSelectionPhase()
    {
        InteractableTile.TilePointerEnter += HighlightUnoccupiedTile;
        InteractableTile.TilePointerExit += ResetAllTileColorsOverload;
        InteractableTile.TilePointerClick += PlacePenguin;
    }

    private void StartGamePhase()
    {
        InteractableTile.TilePointerEnter -= HighlightUnoccupiedTile;
        InteractableTile.TilePointerClick -= PlacePenguin;
        InteractableTile.TilePointerExit -= ResetAllTileColorsOverload;
        
        InteractableTile.TilePointerEnter += InteractableTileHoverEnter;
        InteractableTile.TilePointerClick += InteractableTileClick;
        InteractableTile.TilePointerExit += HighlightSelectable;
    }

    private void HighlightUnoccupiedTile(Location location)
    {
        if (_board.HasTile(location, out var tile) == false) return;
        if (tile.IsOccupied) return;
        if (tile.FishCount != 1) return;
        _colorSystem.ColorTile(location, ColorSystem.ColorType.LightBlue);
    }

    private void PlacePenguin(Location location)
    {
        if (!_board.HasTile(location, out var tile)) return;
        if (tile.IsOccupied) return;
        if (tile.FishCount != 1) return;
        ResetAllTileColors();
        tile.IsOccupied = true;
        tile.Team = Option<Team>.Some(_teamManager.TeamToMove());
        _penguinSystem.PlacePenguin(location, _teamManager.TeamToMove());
        _teamManager.NextTeam();
        _penguinsToPlace--;
        if (_penguinsToPlace == 0) StartGamePhase();
    }

    private void ResetAllTileColors() => 
        _colorSystem.ColorTiles(_board.Tiles
            .Where(tile => tile.IsWater == false)
            .Select(tile => tile.Location), 
            ColorSystem.ColorType.Default);

    private void ResetAllTileColorsOverload(Location _) => ResetAllTileColors();

    private void InteractableTileHoverEnter(Location location)
    {
        if (_board.HasTile(location, out var tile) == false) return;
        if (tile.Team.IsSome(out var team))
        {
            if (team != _teamManager.TeamToMove()) return;
            // Her står min pingvin
            SelectPenguin(location);
            return;
        }
        
        if (!_selectedLocation.IsSome(out var penguinLocation)) return;
        var reachableLocations = _board.ReachableLocations(penguinLocation);
        if (reachableLocations.Contains(location) == false) return;
        _colorSystem.ColorTile(location, ColorSystem.ColorType.Blue);
    }

    private void SelectPenguin(Location location)
    {
        _selectedLocation = Option<Location>.Some(location);
        HighlightSelectable(location);
    }

    private void MoveTo(Location from, Location to)
    {
        if (_board.HasTile(from, out var fromTile) == false) return;
        if (_board.HasTile(to, out var toTile) == false) return;
        fromTile.IsWater = true;
        fromTile.Team = Option<Team>.None;
        _colorSystem.ColorTile(from, ColorSystem.ColorType.Water);
        _pointSystem.AwardPoints(fromTile.FishCount, _teamManager.TeamToMove());
        toTile.IsOccupied = true;
        toTile.Team = Option<Team>.Some(_teamManager.TeamToMove());
        _penguinSystem.MovePenguin(from, to);
        ResetAllTileColors();

        if (_board.TwoPlayersCanMove())
        {
            _teamManager.NextTeam();
            while (_board.HasMovesLeft(_teamManager.TeamToMove()) == false)
            {
                _teamManager.NextTeam();
            }
        }
        else GameOver();
    }

    private void InteractableTileClick(Location location)
    {
        if (_selectedLocation.IsSome(out var penguinLocation) == false) return;
        var reachableLocations = _board.ReachableLocations(penguinLocation);
        if (reachableLocations.Contains(location) == false) return;
        _selectedLocation = Option<Location>.None;
        MoveTo(penguinLocation, location);
    }
    
    private void HighlightSelectable(Location location)
    {
        ResetAllTileColors();
        if (!_selectedLocation.IsSome(out var penguinLocation)) return;
        var reachableLocations = _board.ReachableLocations(penguinLocation);
        _colorSystem.ColorTiles(reachableLocations, ColorSystem.ColorType.LightBlue);
        _colorSystem.ColorTile(penguinLocation, ColorSystem.ColorType.LightBlue);
    }

    private void GameOver()
    {
        InteractableTile.TilePointerEnter -= InteractableTileHoverEnter;
        InteractableTile.TilePointerClick -= InteractableTileClick;
        InteractableTile.TilePointerExit -= HighlightSelectable;

        var remainingRedPoints = _board.RemainingPoints(Team.Red);
        _pointSystem.AwardPoints(remainingRedPoints, Team.Red);
        print($"Red: {remainingRedPoints}");
        var remainingBluePoints = _board.RemainingPoints(Team.Blue);
        _pointSystem.AwardPoints(remainingBluePoints, Team.Blue);
        print($"Blue: {remainingBluePoints}");
    }
}
