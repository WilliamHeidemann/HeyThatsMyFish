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
    [SerializeField] private Sprite waterSprite;
    private Dictionary<Location, SpriteRenderer> _sprites;

    public void CollectSprites()
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
        if (type == ColorType.Water) _sprites[location].sprite = waterSprite;
        else
        {
            _sprites[location].color = type switch
            {
                ColorType.Default => defaultColor,
                ColorType.LightBlue => clickableColor,
                ColorType.Blue => clickedColor,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
    
    public enum ColorType
    {
        Default,
        LightBlue,
        Blue,
        Water
    }
}
