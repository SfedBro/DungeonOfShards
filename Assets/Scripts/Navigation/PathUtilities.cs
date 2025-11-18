using System.Collections.Generic;
using UnityEngine;

namespace Game.Navigation
{
    public static class PathUtilities
    {
        public static List<Vector3> SimplifyPath(List<Node> nodes, LayerMask obstacleMask, float offset = 0.5f)
        {
            List<Vector3> result = new List<Vector3>();
            if (nodes == null || nodes.Count == 0)
                return result;

            int current = 0;
            while (current < nodes.Count - 1)
            {
                int low = current + 1;
                int high = nodes.Count - 1;
                int farthestReachable = low;

                Vector3 start = new Vector3(nodes[current].gridX, offset, nodes[current].gridY);

                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    Vector3 end = new Vector3(nodes[mid].gridX, offset, nodes[mid].gridY);

                    if (!Physics.Linecast(start, end, obstacleMask))
                    {
                        farthestReachable = mid;
                        low = mid + 1; // пробуем дальше
                    }
                    else
                    {
                        high = mid - 1; // слишком далеко
                    }
                }
                result.Add(new Vector3(nodes[farthestReachable].gridX, 0, nodes[farthestReachable].gridY));
                current = farthestReachable;
            }

            return result;
        }
    }
}
