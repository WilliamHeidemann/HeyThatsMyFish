using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using Settings;
using Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

public class  BoardSystem : MonoBehaviour
{
    private Board _board;
    private BoardSpawner _boardSpawner;
    private ColorSystem _colorSystem;
    private PenguinSystem _penguinSystem;
    private PointSystem _pointSystem;
    private TurnManager _turnManager;
    private Option<Location> _selectedLocation = Option<Location>.None;
    private int _penguinsToPlace;
    [SerializeField] private GameSettings gameSettings;
    private void Awake()
    {
        _boardSpawner = FindFirstObjectByType<BoardSpawner>();
        _colorSystem = FindFirstObjectByType<ColorSystem>();
        _penguinSystem = FindFirstObjectByType<PenguinSystem>();
        _pointSystem = FindFirstObjectByType<PointSystem>();
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        _boardSpawner.SpawnBoard(gameSettings.boardType, gameSettings.boardSize);
        _board = gameSettings.boardType switch
        {
            BoardType.Hexagon => Board.CreateHexagonBoard(gameSettings.boardSize),
            BoardType.Square => Board.CreateSquareBoard(gameSettings.boardSize),
            BoardType.Random => Board.CreateRandomBoard(gameSettings.boardSize),
            _ => throw new ArgumentOutOfRangeException()
        };
        _turnManager = new TurnManager(gameSettings.playerCount);
        _pointSystem.SetFishSprites(_board.Tiles);
        _pointSystem.InitializeScoreBoard(gameSettings.playerCount);
        _penguinsToPlace = gameSettings.playerCount * gameSettings.penguinsPerPlayer;
        _penguinSystem.InitializePenguinSystem();
        _colorSystem.CollectSprites();
        StartTileSelectionPhase();
    }

    public void MainMenu() => SceneManager.LoadScene("Main Menu");

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
        tile.Team = Option<Team>.Some(_turnManager.TeamToMove());
        _pointSystem.AwardPoints(tile.FishCount, _turnManager.TeamToMove());
        _penguinSystem.PlacePenguin(location, _turnManager.TeamToMove());
        _turnManager.NextTeam();
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
            if (team != _turnManager.TeamToMove()) return;
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
        _pointSystem.AwardPoints(toTile.FishCount, _turnManager.TeamToMove());
        toTile.IsOccupied = true;
        toTile.Team = Option<Team>.Some(_turnManager.TeamToMove());
        _penguinSystem.MovePenguin(from, to);
        ResetAllTileColors();

        if (_board.TwoPlayersCanMove())
        {
            _turnManager.NextTeam();
            while (_board.HasMovesLeft(_turnManager.TeamToMove()) == false)
            {
                _turnManager.NextTeam();
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
        var remainingBluePoints = _board.RemainingPoints(Team.Blue);
        _pointSystem.AwardPoints(remainingBluePoints, Team.Blue);
        
        _pointSystem.EndGame();
    }
}
