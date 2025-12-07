using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelMap : MonoBehaviour
{
    public int xSize, ySize;

    [Header("Rooms")]
    public MapTile mapTilePrefab;
    public int maxRoomCount = 10;
    public float minRatioXY = 0.2f, maxRatioXY = 0.8f;
    public int maxEnemiesPerRoom = 3;
    [Header("Room Drunkard Walkers")]
    public int drunkardWalkersSteps = 15;
    [Header("Room Diggers")]
    public int maxDiggerSteps = 30;
    public float chanceToPlaceEnemy = 0.05f;
    public float baseChanceChangeDir = 0.05f;
    public float incChanceChangeDir = 0.05f;
    [Header("Room Special Tiles")]
    public float specialTileThreshold = 0.25f;

    MapTileType[,] preTiles;
    MapTile[,] tiles;
    List<Room> rooms;

    readonly List<Vector2Int> dirs = new() { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };

    void Start() {
        GenerateMap();
        AddSpecialTiles();
        SpawnMap();
        SpawnEnemies();
    }

    // shortcut for 2 fors
    void IterateOver2Loops(int n1, int n2, Action<int, int> action) {
        for (int i1 = 0; i1 < n1; ++i1)
            for (int i2 = 0; i2 < n2; ++i2)
                action(i1, i2);
    }
    
    void GenerateMap() {
        // destroy current map if exists and create new one
        if (tiles != null) IterateOver2Loops(tiles.GetLength(0), tiles.GetLength(1), (x, y) => {
            MapTile tile = tiles[x, y];
            if (tile != null) Destroy(tile.gameObject);
        });
        // matrix for storing map tiles types, before creating actual types
        preTiles = new MapTileType[xSize, ySize];
        // default tile types to wall
        IterateOver2Loops(xSize, ySize, (x, y) => preTiles[x, y] = MapTileType.Wall);
        int[] corners = new int[4] {-xSize / 2, -ySize / 2, (xSize + 1) / 2, (ySize + 1) / 2}; // map corners
        rooms = MapSpacePartitioning.GenerateRooms(corners, maxRoomCount, minRatioXY, maxRatioXY); // generates room points
        print(rooms.Count);
        rooms.ForEach(r => print(r));
        for (int i = 0; i < rooms.Count; ++i) {
            Room room = rooms[i];
            int xMid = room.x + room.xLen / 2, yMid = room.y + room.yLen / 2;
            List<Vector2Int> walkers = new() { new(xMid + 1, yMid + 1), new(xMid - 1, yMid + 1), new(xMid + 1, yMid - 1), new(xMid - 1, yMid - 1) };
            for (int iter = 0; iter < drunkardWalkersSteps; iter++) {
                for (int w = 0; w < walkers.Count; ++w) {
                    Vector2Int walker = walkers[w];
                    preTiles[walker.x, walker.y] = MapTileType.Empty;
                    Vector2Int dir = dirs.SelectRandom();
                    Vector2Int newWalker = walker + dir;
                    if (newWalker.x >= room.x && newWalker.x < room.XMax() && newWalker.y >= room.y && newWalker.y < room.YMax()) walkers[w] = newWalker;
                }
            }
        }
    }

    // get map tile by coords
    public MapTile GetMapTile(int x, int y) {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize) return null;
        return tiles[x, y];
    }

    // get matrix of map tile types
    public MapTileType[,] GetMapTileTypes() {
        MapTileType[,] types = new MapTileType[xSize, ySize];
        IterateOver2Loops(xSize, ySize, (x, y) => {
            MapTile tile = tiles[x, y];
            types[x, y] = tile == null ? MapTileType.Empty : tile.mapTileType;
        });
        return types;
    }

    // spawn enemies on generated map
    void SpawnEnemies() {
        List<int> changeDirOffset = new() { 1, -1 };
        // for every room
        for (int i = 0; i < rooms.Count; ++i) {
            Room room = rooms[i];
            // spawn digger
            Vector2Int digger = new(Random.Range(room.x, room.XMax()), Random.Range(room.y, room.YMax()));
            int dirInd = Random.Range(0, dirs.Count);
            Vector2Int dir = dirs[dirInd];
            float chanceChangeDir = baseChanceChangeDir;
            // terminal criteries
            int placedEnemies = 0, steps = 0;
            while (placedEnemies < maxEnemiesPerRoom && steps < maxDiggerSteps) {
                ++steps;
                // move digger
                Vector2Int newDiggerPos = digger + dir;
                bool forceNewDir = false;
                if (newDiggerPos.x < room.x || newDiggerPos.x >= room.XMax() || newDiggerPos.y < room.y || newDiggerPos.y >= room.YMax()) forceNewDir = true;
                else digger = newDiggerPos;
                // change direction if needed
                if (forceNewDir || Random.value <= chanceChangeDir) {
                    dirInd = (dirInd + changeDirOffset.SelectRandom() + dirs.Count) % dirs.Count;
                    dir = dirs[dirInd];
                } else chanceChangeDir += incChanceChangeDir;
                // spawn enemy if needed
                if (Random.value <= chanceToPlaceEnemy) {
                    // place enemy
                    ++placedEnemies;
                }
            }
        }
    }

    // add special tiles to map
    void AddSpecialTiles() {
        PerlinNoise pn = new(xSize, ySize);
        IterateOver2Loops(xSize, ySize, (x, y) => {
            float n = pn.Get(x, y);
            if (preTiles[x, y] == MapTileType.Empty && n >= specialTileThreshold) preTiles[x, y] = MapTileType.Slow;
        });
    }

    // spawn real tiles
    void SpawnMap() {
        // init matrix
        tiles = new MapTile[xSize, ySize];
        IterateOver2Loops(xSize, ySize, (x, y) => {
            MapTile tile = Instantiate(mapTilePrefab, new(x, y, 0), Quaternion.identity, transform);
            tile.mapTileType = preTiles[x, y];
            tiles[x, y] = tile;
        });
    }
}

public static class ListUtils {
    public static T SelectRandom<T>(this IEnumerable<T> values) {
        return values.ElementAt(Random.Range(0, values.Count()));
    }
}

class PerlinNoise {
    Vector2[,] gradients;
    int xSize, ySize;

    public PerlinNoise(int xSize, int ySize) {
        this.xSize = xSize; this.ySize = ySize;
        gradients = new Vector2[xSize + 1, ySize + 1];
        for (int x = 0; x < xSize + 1; ++x)
            for (int y = 0; y < ySize + 1; ++y)
                gradients[x, y] = Quaternion.Euler(0, 0, Random.Range(0f, 360f)) * Vector2.up;
    }

    public float Get(int x, int y) {
        Vector2
            g00 = gradients[x, y],
            g01 = gradients[x, y + 1],
            g10 = gradients[x + 1, y],
            g11 = gradients[x + 1, y + 1];
        float
            dot00 = Dot(g00, 0.5f, 0.5f),
            dot01 = Dot(g01, 0.5f, -0.5f),
            dot10 = Dot(g10, -0.5f, 0.5f),
            dot11 = Dot(g11, -0.5f, -0.5f);
        float yl1 = Mathf.Lerp(dot00, dot01, 0.5f), yl2 = Mathf.Lerp(dot10, dot11, 0.5f);
        return Mathf.Lerp(yl1, yl2, 0.5f);
    }

    float Dot(Vector2 v, float dx, float dy) {
        return v.x * dx + v.y * dy;
    }
}