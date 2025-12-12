using UnityEngine;

namespace Game.Navigation
{
    public class NavigationGrid : MonoBehaviour
    {
        public float nodeSize = 1f;
        private MapTileType[,] tiles;
        private int sizeX;
        private int sizeY;
        private Vector3 origin;

        public LevelMap levelMap;

        void Awake()
        {
            Init(levelMap);
        }

        public void Init(LevelMap map)
        {
            tiles = map.GetMapTileTypes();
            sizeX = tiles.GetLength(0);
            sizeY = tiles.GetLength(1);
            origin = transform.position;
        }
        public bool IsWalkable(int x, int y)
        {
            if (x < 0 || x >= sizeX || y < 0 || y >= sizeY)
                return false;

            MapTileType t = tiles[x, y];
            return t == MapTileType.Empty || t == MapTileType.Slow;
        }

        public Vector2Int WorldToGrid(Vector3 pos)
        {
            int x = Mathf.FloorToInt((pos.x - origin.x) / nodeSize);
            int y = Mathf.FloorToInt((pos.y - origin.y) / nodeSize);
            return new Vector2Int(x, y);
        }

        public Vector3 GridToWorld(int x, int y)
        {
            return origin + new Vector3(x * nodeSize, y * nodeSize, 0);
        }

        public Vector3 GridToWorld(Vector2Int p)
        {
            return GridToWorld(p.x, p.y);
        }

        public MapTileType[,] GetGrid()
        {
            if (tiles == null)
            {
                Debug.LogError("NavigationGrid tiles == null: Init() not called yet!");
            }
            return tiles;
        }
        public int Width => sizeX;
        public int Height => sizeY;
    }
}
