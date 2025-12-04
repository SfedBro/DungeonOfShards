using System.Collections.Generic;
using UnityEngine;

namespace Game.Navigation
{
    public class Pathfinding
    {
        private readonly NavigationGrid grid;

        public Pathfinding(NavigationGrid grid)
        {
            this.grid = grid;
        }

        public List<Node> FindPath(Vector3 startPos, Vector3 targetPos, MovementType movementType)
        {
            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node targetNode = grid.NodeFromWorldPoint(targetPos);
            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < current.fCost ||
                        openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost)
                        current = openSet[i];
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == targetNode)
                    return RetracePath(startNode, targetNode);

                foreach (Node neighbor in GetNeighbors(current))
                {
                    if (!neighbor.free || closedSet.Contains(neighbor))
                        continue;
                    if (neighbor.allowedMovement == movementType)
                        continue;

                    float newCost = current.gCost + Vector3.Distance(GridToWorld(current), GridToWorld(neighbor));
                    if (newCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCost;
                        neighbor.hCost = Vector3.Distance(GridToWorld(neighbor), GridToWorld(targetNode));
                        neighbor.parent = current;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private List<Node> RetracePath(Node start, Node end)
        {
            List<Node> path = new List<Node>();
            Node current = end;
            while (current != start)
            {
                path.Add(current);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        private IEnumerable<Node> GetNeighbors(Node node)
        {
            Node[,] gridData = grid.GetGrid();
            int x = node.gridX;
            int y = node.gridY;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < gridData.GetLength(0) && ny < gridData.GetLength(1))
                        yield return gridData[nx, ny];
                }
            }
        }

        private Vector3 GridToWorld(Node node)
        {
            return new Vector3(node.gridX, 0, node.gridY);
        }
    }
}
