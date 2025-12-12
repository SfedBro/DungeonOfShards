using System.Collections.Generic;
using UnityEngine;

namespace Game.Navigation
{
    public static class PathUtilities
    {
        public static List<Vector3> SimplifyPath(List<Vector2Int> path, NavigationGrid grid)
        {
            List<Vector3> result = new ();
            if (path == null || path.Count == 0)
                return result;

            int current = 0;
            while (current < path.Count - 1)
            {
                int low = current + 1;
                int high = path.Count - 1;
                int farthestReachable = low;

                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    if (HasLineOfSight(grid, path[current], path[mid]))
                    {
                        farthestReachable = mid;
                        low = mid + 1;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }
                result.Add(grid.GridToWorld(path[farthestReachable]));
                current = farthestReachable;
            }

            return result;
        }
        private static bool HasLineOfSight(NavigationGrid grid, Vector2Int a, Vector2Int b)
        {
            int x0 = a.x;
            int y0 = a.y;
            int x1 = b.x;
            int y1 = b.y;

            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;

            int err = dx - dy;

            while (true)
            {
                if (!grid.IsWalkable(x0, y0)) return false;

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;

                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return true;
        }
    }
}
