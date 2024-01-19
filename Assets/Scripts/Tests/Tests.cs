using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Tests
{
    private Board _testHexBoard;
    private Board _testSquareBoard;
    private List<Tile> _testPath;
    
    [SetUp]
    public void SetUp()
    {
        _testHexBoard = Board.CreateHexagonBoard(5);
        _testSquareBoard = Board.CreateSquareBoard(5);
        var location1 = new Location(0, 0, 0);
        var location2 = new Location(1, 0, -1);
        var location3 = new Location(2, 0, -2);
        var location4 = new Location(3, 0, -3);

        var locations = new List<Location>
        {
            location1,
            location2,
            location3,
            location4,
        };
        _testPath = locations.Select(location => new Tile(location)).ToList();
    }

    [Test]
    [TestCase(0,0,0,+3,0,-3)]
    [TestCase(0,0,0,0,+2,-2)]
    [TestCase(0,0,0,-1,+1,0)]
    public void LocationsAreInTheSameLine(int q1, int r1, int s1, int q2, int r2, int s2)
    {
        var location1 = new Location(q1, r1, s1);
        var location2 = new Location(q2, r2, s2);
        Assert.True(location1.IsSameLine(location2));
    }
    
    [Test]
    [TestCase(0,-1,+2,+3,0,-3)]
    [TestCase(-1,0,+1,0,+2,-2)]
    [TestCase(+2,0,-2,-1,+1,0)]
    public void LocationsAreNotInTheSameLine(int q1, int r1, int s1, int q2, int r2, int s2)
    {
        var location1 = new Location(q1, r1, s1);
        var location2 = new Location(q2, r2, s2);
        Assert.False(location1.IsSameLine(location2));
    }

    [Test]
    public void PathBlockedByPenguin_ReturnsIsBlocked()
    {
        _testPath.First(tile => tile.Location.Q == 2).IsOccupied = true;
        var isBlocked = _testPath.IsBlocked();
        Assert.True(isBlocked);
    }

    [Test]
    public void PathBlockedByWater_ReturnsIsBlocked()
    {
        _testPath.First(tile => tile.Location.Q == 2).IsWater = true;
        var isBlocked = _testPath.IsBlocked();
        Assert.True(isBlocked);
    }
    
    [Test]
    public void PathNotBlocked_ReturnsIsNotBlocked()
    {
        var isBlocked = _testPath.IsBlocked();
        Assert.False(isBlocked);
    }

    [Test]
    public void GetPathReturnsCorrectPath()
    {
        var source = new Location(0, 0, 0);
        var destination = new Location(3, 0, -3);
        var path = _testHexBoard.GetPath(source, destination);
        Assert.Equals(_testPath, path);
    }

    [Test]
    public void ReachableTileCountFromOriginIs24()
    {
        var source = new Location(0, 0, 0);
        var tiles = _testHexBoard.ReachableLocations(source);
        var reachableCount = tiles.Count();
        Assert.AreEqual(24, reachableCount);
    }
    
    [Test]
    public void ReachableTileCountFromOriginWithOccupiedTileIs20()
    {
        var source = new Location(0, 0, 0);
        _testHexBoard.Tiles.First(tile => tile.Location.Q == 0 && tile.Location.R == 1).IsOccupied = true;
        var tiles = _testHexBoard.ReachableLocations(source);
        var reachableCount = tiles.Count();
        Assert.AreEqual(20, reachableCount);
    }
}
