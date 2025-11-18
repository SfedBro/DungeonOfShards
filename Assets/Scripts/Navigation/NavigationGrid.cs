using UnityEngine;

namespace Game.Navigation
{
    public class NavigationGrid : MonoBehaviour
    {
        public bool vertical = true;
        public int gridSizeX = 20;
        public int gridSizeY = 20;
        public float nodeSize = 1f;
        public LayerMask obstacleMask;

        private Node[,] grid;

        public void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 origin = transform.position;

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = origin + Vector3.right * (x * nodeSize + nodeSize / 2) + Vector3.forward * (y * nodeSize + nodeSize / 2);
                    bool notOccupied = !Physics.CheckSphere(worldPoint, nodeSize * 0.4f, obstacleMask);
                    MovementType movement = MovementType.Ground | MovementType.Flying;
                    grid[x, y] = new Node(x, y, notOccupied, movement);
                }
            }
        }

        public Node NodeFromWorldPoint(Vector3 worldPos)
        {
            float percentX = Mathf.Clamp01((worldPos.x + gridSizeX / 2f * nodeSize) / (gridSizeX * nodeSize));
            float percentY = Mathf.Clamp01((worldPos.z + gridSizeY / 2f * nodeSize) / (gridSizeY * nodeSize));
            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
            return grid[x, y];
        }

        public Node[,] GetGrid() => grid;
    }
}
