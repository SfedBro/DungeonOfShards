using System.Collections.Generic;
using UnityEngine;

namespace Game.Navigation
{
    public class Pathfinding
    {
        private NavigationGrid grid;
        private int sizeX;
        private int sizeY;

        private class Node
        {
            public int x;
            public int y;
            public float gCost;
            public float hCost;
            public Node parent;

            public float fCost
            {
                get { return gCost + hCost; }
            }

            public Node(int x, int y)
            {
                this.x = x;
                this.y = y;
                gCost = float.MaxValue;
                hCost = 0;
                parent = null;
            }
        }

        public Pathfinding(NavigationGrid grid)
        {
            this.grid = grid;
            var g = grid.GetGrid();
            sizeX = g.GetLength(0);
            sizeY = g.GetLength(1);
        }

        public List<Vector2Int> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Vector2Int start = grid.WorldToGrid(startPos);
            Vector2Int end = grid.WorldToGrid(targetPos);
            if (!grid.IsWalkable(start.x, start.y) || !grid.IsWalkable(end.x, end.y))
                return null;

            Node[,] nodes = new Node[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    nodes[x, y] = new Node(x, y);
                }
            }
            Node startNode = nodes[start.x, start.y];
            Node endNode = nodes[end.x, end.y];
            startNode.gCost = 0f;

            List<Node> open = new ();
            bool[,] closed = new bool[sizeX, sizeY];

            open.Add(startNode);

            while (open.Count > 0)
            {
                Node current = open[0];
                for (int i = 1; i < open.Count; i++)
                {
                    if (open[i].fCost < current.fCost || (open[i].fCost == current.fCost && open[i].hCost < current.hCost))
                    {
                        current = open[i];
                    }
                }

                open.Remove(current);
                closed[current.x, current.y] = true;

                if (current == endNode)
                    return BuildPath(endNode);

                foreach (Node n in GetNeighbors(nodes, current))
                {
                    if (!grid.IsWalkable(n.x, n.y) || closed[n.x, n.y])
                        continue;

                    float newCost = current.gCost + Distance(current, n);
                    if (newCost < n.gCost)
                    {
                        n.gCost = newCost;
                        n.hCost = Distance(n, endNode);
                        n.parent = current;

                        if (!open.Contains(n))
                            open.Add(n);
                    }
                }
            }
            return null;
        }

        private List<Vector2Int> BuildPath(Node endNode)
        {
            List<Vector2Int> path = new ();
            Node current = endNode;
            while (current != null)
            {
                path.Add(new Vector2Int(current.x, current.y));
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        private IEnumerable<Node> GetNeighbors(Node[,] nodes, Node n)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = n.x + dx;
                    int ny = n.y + dy;
                    if (nx >= 0 && ny >= 0 && nx < sizeX && ny < sizeY)
                        yield return nodes[nx, ny];
                }
            }
        }
        private float Distance(Node a, Node b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
    }
}
