using System.Collections.Generic;
using UnityEngine;

namespace Game.Navigation
{
    public class PathfindingController : MonoBehaviour
    {
        public NavigationGrid grid;
        public Pathfinding pathfinder;

        public void Init(NavigationGrid grid)
        {
            pathfinder = new Pathfinding(grid);
        }

        public List<Vector3> BuildPath(Vector3 from, Vector3 to)
        {
            var raw = pathfinder.FindPath(from, to);
            if (raw == null)
            {
                Debug.Log("FindPath = NULL");
                return null;
            }
            Debug.Log("FindPath raw len: " + raw.Count);

            var simplified = PathUtilities.SimplifyPath(raw, grid);

            Debug.Log("Simplify len: " + simplified.Count);

            return simplified;
        }
    }
}

