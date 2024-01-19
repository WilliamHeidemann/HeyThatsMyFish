using System;
using System.Collections;
using System.Collections.Generic;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InteractableTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Location location;

    public static Action<Location> TilePointerEnter;
    public static Action<Location> TilePointerExit;
    public static Action<Location> TilePointerClick;

    public void OnPointerEnter(PointerEventData eventData) => TilePointerEnter?.Invoke(location);

    public void OnPointerExit(PointerEventData eventData) => TilePointerExit?.Invoke(location);

    public void OnPointerClick(PointerEventData eventData) => TilePointerClick?.Invoke(location);
}
