using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;

public class BoardSystem : MonoBehaviour
{
    public Board Board;
    public readonly Dictionary<Location, SpriteRenderer> TileSprites = new();
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Color hoverColor;

    public void InteractableTileHoverEnter(Location location)
    {
        TileSprites[location].color = hoverColor;
    }
    
    public void InteractableTileHoverExit(Location location)
    {
        TileSprites.Values.ToList().ForEach(sprite => sprite.color = defaultColor);
    }
    
    public void InteractableTileClick(Location location)
    {
        TileSprites[location].color = selectColor;
        Board.ReachableTiles(location).ToList().ForEach(tile => TileSprites[tile.Location].color = hoverColor);
    }
}
