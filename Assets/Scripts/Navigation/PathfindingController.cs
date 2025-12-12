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
            if (raw == null) return null;

            List<Vector3> rawWorld = new List<Vector3>();
            foreach (var p in raw)
                rawWorld.Add(grid.GridToWorld(p));

            return PathUtilities.SimplifyPath(raw, grid);
        }
    }
}

