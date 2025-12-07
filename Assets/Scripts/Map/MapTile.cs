using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MapTile : MonoBehaviour
{
    public int x, y;
    public SerializedDictionary<MapTileType, Color> mapTileColors;
    public MapTileType mapTileType;

    SpriteRenderer sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update() {
        Color shouldColor = mapTileColors.GetValueOrDefault(mapTileType, Color.black);
        if (sprite.color != shouldColor) sprite.color = shouldColor;
    }
}

public enum MapTileType
{
    Empty,
    Wall,
    Slow
}