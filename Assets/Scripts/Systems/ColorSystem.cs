using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;

public class ColorSystem : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color clickableColor;
    [SerializeField] private Color clickedColor;
    private Dictionary<Location, SpriteRenderer> _sprites;

    private void Start()
    {
        _sprites = FindObjectsByType<InteractableTile>(FindObjectsSortMode.None)
            .ToDictionary(tile => tile.location, tile => tile.GetComponent<SpriteRenderer>());
    }


    public void ColorTiles(IEnumerable<Location> locations, ColorType type)
    {
        foreach (var location in locations)
        {
            ColorTile(location, type);
        }
    }
    
    public void ColorTile(Location location, ColorType type)
    {
        if (_sprites.ContainsKey(location) == false) return;
        _sprites[location].color = type switch
        {
            ColorType.Default => defaultColor,
            ColorType.Clickable => clickableColor,
            ColorType.Clicked => clickedColor,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
    
    public enum ColorType
    {
        Default,
        Clickable,
        Clicked,
    }
}
