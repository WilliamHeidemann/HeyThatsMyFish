using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Location Location;
    [HideInInspector] public BoardSystem boardSystem;

    private void Start()
    {
        boardSystem.TileSprites.Add(Location, GetComponent<SpriteRenderer>());
    }

    public void OnPointerEnter(PointerEventData eventData) => boardSystem.InteractableTileHoverEnter(Location);

    public void OnPointerExit(PointerEventData eventData) => boardSystem.InteractableTileHoverExit(Location);

    public void OnPointerClick(PointerEventData eventData) => boardSystem.InteractableTileClick(Location);
}
