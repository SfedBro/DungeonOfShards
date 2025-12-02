using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelMap : MonoBehaviour
{
    public int xSize, ySize;

    [Header("Rooms")]
    public int maxEnemiesPerRoom = 3;
    public int maxDiggerSteps = 30;
    public float chanceToPlaceEnemy = 0.05f;
    public float baseChanceChangeDir = 0.05f;
    public float incChanceChangeDir = 0.05f;

    MapTile[,] tiles;

    void Start() {
        GenerateMap();
        SpawnEnemies();
    }

    void IterateOver2Loops(int n1, int n2, Action<int, int> action) {
        for (int i1 = 0; i1 < n1; ++i1)
            for (int i2 = 0; i2 < n2; ++i2)
                action(i1, i2);
    }
    
    void GenerateMap() {
        IterateOver2Loops(tiles.GetLength(0), tiles.GetLength(1), (x, y) => {
            MapTile tile = tiles[x, y];
            if (tile != null) Destroy(tile.gameObject);
        });
        tiles = new MapTile[xSize, ySize];
    }

    public MapTile GetMapTile(int x, int y) {
        if (x < 0 || x >= xSize || y < 0 || y >= ySize) return null;
        return tiles[x, y];
    }

    public MapTileType[,] GetTileMapTypes() {
        MapTileType[,] types = new MapTileType[xSize, ySize];
        IterateOver2Loops(xSize, ySize, (x, y) => {
            MapTile tile = tiles[x, y];
            types[x, y] = tile == null ? MapTileType.Empty : tile.mapTileType;
        });
        return types;
    }

    struct Room {
        public int x, y, xLen, yLen;
    }
    void SpawnEnemies() {
        List<Room> rooms = new(); // get rooms
        List<Vector2Int> dirs = new() { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };
        List<int> changeDirOffset = new() { 1, -1 };
        // for every room
        for (int i = 0; i < rooms.Count; i++) {
            Room room = rooms[i];
            // spawn digger
            Vector2Int digger = new(Random.Range(room.x, room.x + room.xLen), Random.Range(room.y, room.y + room.yLen));
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
                if (newDiggerPos.x < room.x || newDiggerPos.x >= room.x + room.xLen || newDiggerPos.y < room.y || newDiggerPos.y >= room.y + room.yLen) forceNewDir = true;
                else digger = newDiggerPos;
                // change direction if needed
                if (forceNewDir || Random.value <= chanceChangeDir) {
                    dirInd = (dirInd + changeDirOffset[Random.Range(0, changeDirOffset.Count)] + dirs.Count) % dirs.Count;
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
