using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelMap : MonoBehaviour
{
    public int xSize, ySize;

    [Header("Rooms")]
    public int maxRoomCount = 10;
    public int maxEnemiesPerRoom = 3;
    public int maxDiggerSteps = 30;
    public float chanceToPlaceEnemy = 0.05f;
    public float baseChanceChangeDir = 0.05f;
    public float incChanceChangeDir = 0.05f;

    MapTileType[,] preTiles;
    MapTile[,] tiles;
    List<Room> rooms;

    List<Vector2Int> dirs = new() { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };
    struct Room {
        public int x, y, xLen, yLen;
        public int xMax() { return x + xLen; }
        public int yMax() { return y + yLen; }
    }

    void Start() {
        GenerateMap();
        SpawnEnemies();
    }

    void IterateOver2Loops(int n1, int n2, Action<int, int> action) {
        for (int i1 = 0; i1 < n1; ++i1)
            for (int i2 = 0; i2 < n2; ++i2)
                action(i1, i2);
    }
    
    // destroy current map if exists and create new one
    void GenerateMap() {
        IterateOver2Loops(tiles.GetLength(0), tiles.GetLength(1), (x, y) => {
            MapTile tile = tiles[x, y];
            if (tile != null) Destroy(tile.gameObject);
        });
        preTiles = new MapTileType[xSize, ySize];
        IterateOver2Loops(xSize, ySize, (x, y) => preTiles[x, y] = MapTileType.Wall);
        rooms = new(); // TODO: generate room points
        for (int i = 0; i < rooms.Count; ++i) {
            Room room = rooms[i];
            int xMid = room.x + room.xLen / 2, yMid = room.y + room.yLen / 2;
            List<Vector2Int> diggers = new() { new(xMid + 1, yMid + 1), new(xMid - 1, yMid + 1), new(xMid + 1, yMid - 1), new(xMid - 1, yMid - 1) };
            for (int d = 0; d < diggers.Count; ++d) {
                Vector2Int digger = diggers[d];
                preTiles[digger.x, digger.y] = MapTileType.Empty;
                Vector2Int dir = dirs.SelectRandom();
                Vector2Int newDigger = digger + dir;
                if (newDigger.x < room.x || newDigger.x >= room.xMax() || newDigger.y < room.y || newDigger.y >= room.yMax()) diggers[d] = newDigger;
            }
        }
        // TODO: generate special tiles
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

    void SpawnEnemies() {
        List<int> changeDirOffset = new() { 1, -1 };
        // for every room
        for (int i = 0; i < rooms.Count; ++i) {
            Room room = rooms[i];
            // spawn digger
            Vector2Int digger = new(Random.Range(room.x, room.xMax()), Random.Range(room.y, room.yMax()));
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
                if (newDiggerPos.x < room.x || newDiggerPos.x >= room.xMax() || newDiggerPos.y < room.y || newDiggerPos.y >= room.yMax()) forceNewDir = true;
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
}

public static class ListUtils {
    public static T SelectRandom<T>(this IEnumerable<T> values) {
        return values.ElementAt(Random.Range(0, values.Count()));
    }
}
