using UnityEngine;

public class MapTile : MonoBehaviour
{
    public int x, y;
    public MapTileType mapTileType; 
}

public enum MapTileType
{
    Empty,
    Wall,
    Slow
}