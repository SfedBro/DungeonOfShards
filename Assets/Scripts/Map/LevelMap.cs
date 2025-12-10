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
    [Header("Room Drunkard Walkers")]
    public float roomEmptyFraction = 0.8f; // empty tiles * roomEmptyFraction >= all room tiles
    [Header("Room Diggers")]
    public int maxDiggerSteps = 30;
    public float chanceToPlaceEnemy = 0.05f;
    public float baseChanceChangeDir = 0.05f;
    public float incChanceChangeDir = 0.05f;
    [Header("Room Special Tiles")]
    public float specialTileThreshold = 0.25f;
    [Header("Enemies")]
    public int maxEnemiesPerRoom = 3;
    public GameObject enemyPrefab;


    MapTileType[,] preTiles;
    MapTile[,] tiles;
    List<Room> rooms;
    public List<int> roomsView;

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
        int[] corners = new int[4] {0, 0, xSize, ySize}; // map corners
        rooms = MapSpacePartitioning.GenerateRooms(corners, maxRoomCount, minRatioXY, maxRatioXY); // generates room points
        int ind = 0;
        foreach (Room r in rooms) roomsView.Add(ind++);
        foreach (Room room in rooms) {
            print($"room {room.x} {room.y} {room.xLen} {room.yLen}");
            int xMid = room.x + room.xLen / 2, yMid = room.y + room.yLen / 2;
            Vector2Int roomMin = new(room.x + 1, room.y + 1), roomMax = new(room.XMax() - 2, room.YMax() - 2);
            // spawning drunkard walkers
            List<Vector2Int> walkers = new List<Vector2Int>{ new(xMid + 1, yMid + 1), new(xMid - 1, yMid + 1), new(xMid + 1, yMid - 1), new(xMid - 1, yMid - 1) }.Select(w => w.ClampRet(roomMin, roomMax)).ToList();
            HashSet<Vector2Int> emptyTiles = new();
            int minimumEmptyTiles = (int)((room.xLen - 2) * (room.yLen - 2) * roomEmptyFraction);
            print(minimumEmptyTiles);
            // termination criteria
            while (emptyTiles.Count < minimumEmptyTiles) {
                for (int w = 0; w < walkers.Count; ++w) {
                    Vector2Int walker = walkers[w];
                    // walker changes wall tile to empty
                    preTiles[walker.x, walker.y] = MapTileType.Empty;
                    emptyTiles.Add(walker);
                    // then walker tries to walk
                    Vector2Int dir = dirs.SelectRandom();
                    Vector2Int newWalker = walker + dir;
                    // walker can be only in his room
                    if (newWalker.x >= roomMin.x && newWalker.x <= roomMax.x && newWalker.y >= roomMin.y && newWalker.y <= roomMax.y) walkers[w] = newWalker;
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
            Vector2Int roomMin = new(room.x + 1, room.y + 1), roomMax = new(room.XMax() - 2, room.YMax() - 2);
            // spawn digger
            Vector2Int digger = new(Random.Range(roomMin.x, room.XMax() - 2), Random.Range(roomMax.y, room.YMax() - 2));
            int dirInd = Random.Range(0, dirs.Count);
            Vector2Int dir = dirs[dirInd];
            float chanceChangeDir = baseChanceChangeDir;
            // terminal criteries
            int placedEnemies = 0, steps = 0;
            bool needToPlace = false;
            while (placedEnemies < maxEnemiesPerRoom && steps < maxDiggerSteps) {
                ++steps;
                // move digger
                Vector2Int newDiggerPos = digger + dir;
                bool forceNewDir = false;
                if (newDiggerPos.x < roomMin.x || newDiggerPos.x > roomMax.x || newDiggerPos.y < roomMin.y || newDiggerPos.y > roomMax.y) forceNewDir = true;
                else digger = newDiggerPos;
                // change direction if needed
                if (forceNewDir || Random.value <= chanceChangeDir) {
                    dirInd = (dirInd + changeDirOffset.SelectRandom() + dirs.Count) % dirs.Count;
                    dir = dirs[dirInd];
                } else chanceChangeDir += incChanceChangeDir;
                // try to place enemy if needed
                if (Random.value <= chanceToPlaceEnemy) needToPlace = true;
                if (needToPlace && preTiles[digger.x, digger.y] == MapTileType.Empty) {
                    needToPlace = false;
                    // place enemy
                    Instantiate(enemyPrefab, (Vector3Int)digger, Quaternion.identity);
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

public static class VectorUtils {
    public static Vector2 ClampRet(this Vector2 v, Vector2 vMin, Vector2 vMax) {
        return new(Mathf.Clamp(v.x, vMin.x, vMax.x), Mathf.Clamp(v.y, vMin.y, vMax.y));
    }

    public static Vector2Int ClampRet(this Vector2Int v, Vector2Int vMin, Vector2Int vMax) {
        return new(Mathf.Clamp(v.x, vMin.x, vMax.x), Mathf.Clamp(v.y, vMin.y, vMax.y));
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