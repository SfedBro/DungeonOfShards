using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ModifiedAStarCorridors
{
    public static List<Vector2Int> MapCorridors(int[] corners, List<Room> rooms, int width = 1)
    {
        HashSet<Vector2Int> roomCenters = new (rooms.Count);
        foreach (Room room in rooms)
        {
            Vector2Int rc = new(room.x + room.xLen / 2, room.y + room.yLen / 2);
            roomCenters.Add(rc);
        }
        var map = MakeMapLayout(corners[2] - corners[0], corners[3] - corners[1], roomCenters);

        List<Vector2Int> centers = roomCenters.ToList();
        List<Vector2Int> corp = new List<Vector2Int>(); //corridor paths

        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            Vector2Int a = centers[i];
            Vector2Int b = centers[i + 1];

            var path = AStarPath(map, a, b);

            if (path.Count == 0)
            {
                // A* could not find a single way
                // fallback to right angled straight corridor to the room
                path = FallbackDig(a, b);
            }

            // applying width(obvious, right?)
            var wide = ApplyWidth(path, width);
            corp.AddRange(wide);

            // noting corridor cells as ones
            foreach (var p in wide)
                map[p.x, p.y] = 1;
        }
        return corp;
    }
    public static int[,] MakeMapLayout(int w, int h, HashSet<Vector2Int> rooms)
    {
        int[,] map = new int[w, h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                map[i, j] = rooms.Contains(new Vector2Int(i, j)) ? 1 : 0;
            }
        }
        return map;
    }
    public static List<Vector2Int> AStarPath(int[,] map, Vector2Int start, Vector2Int goal)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);

        bool InBounds(int x, int y)
        {
            return x >= 0 && x < w && y >= 0 && y < h;
        }

        var open = new PriorityQueue<Vector2Int>();
        open.Enqueue(start, 0);

        var came = new Dictionary<Vector2Int, Vector2Int>();
        var g = new Dictionary<Vector2Int, int>();
        g[start] = 0;

        Vector2Int[] dirs = new Vector2Int[] { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };

        while (open.Count > 0)
        {
            Vector2Int current = open.Dequeue();

            if (current == goal)
                return AssemblePath(came, current);

            foreach (var d in dirs)
            {
                Vector2Int next = new Vector2Int(current.x + d.x, current.y + d.y);

                if (!InBounds(next.x, next.y))
                    continue;

                int tentative = g[current] + 1;

                if (!g.ContainsKey(next) || tentative < g[next])
                {
                    g[next] = tentative;

                    int hCost = Mathf.Abs(next.x - goal.x) + Mathf.Abs(next.y - goal.y);
                    int fCost = tentative + hCost;

                    open.Enqueue(next, fCost);
                    came[next] = current;
                }
            }
        }
        return new List<Vector2Int>();
    }
    private static List<Vector2Int> AssemblePath(Dictionary<Vector2Int, Vector2Int> came, Vector2Int cur)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        while (came.ContainsKey(cur))
        {
            path.Add(cur);
            cur = came[cur];
        }
        path.Add(cur);
        path.Reverse();
        return path;
    }
    public static List<Vector2Int> FallbackDig(Vector2Int a, Vector2Int b)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        int x = a.x;
        int y = a.y;

        while (x != b.x)
        {
            result.Add(new Vector2Int(x, y));
            x += x < b.x ? 1 : -1;
        }

        while (y != b.y)
        {
            result.Add(new Vector2Int(x, y));
            y += y < b.y ? 1 : -1;
        }

        result.Add(b);
        return result;
    }
    public static List<Vector2Int> ApplyWidth(List<Vector2Int> path, int corridorWidth) // circular shape width
    {
        HashSet<Vector2Int> result = new();

        int r = corridorWidth / 2;

        foreach (var p in path)
        {
            for (int dx = -r; dx <= r; dx++)
            {
                int maxDy = Mathf.FloorToInt(Mathf.Sqrt(r * r - dx * dx));
                for (int dy = -maxDy; dy <= maxDy; dy++)
                {
                    Vector2Int v = new Vector2Int(p.x + dx, p.y + dy);
                    result.Add(v);
                }
            }
        }
        return new List<Vector2Int>(result);
    }
}
public class PriorityQueue<T>
{
    private readonly List<(T item, int priority)> heap = new();

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        int c = heap.Count - 1;
        while (c > 0)
        {
            int p = (c - 1) / 2;
            if (heap[c].priority >= heap[p].priority) break;
            (heap[c], heap[p]) = (heap[p], heap[c]);
            c = p;
        }
    }

    public T Dequeue()
    {
        int last = heap.Count - 1;
        (heap[0], heap[last]) = (heap[last], heap[0]);
        var result = heap[last].item;
        heap.RemoveAt(last);

        int p = 0;
        while (true)
        {
            int l = p * 2 + 1;
            int r = p * 2 + 2;

            if (l >= heap.Count) break;

            int c = (r < heap.Count && heap[r].priority < heap[l].priority) ? r : l;

            if (heap[p].priority <= heap[c].priority) break;

            (heap[p], heap[c]) = (heap[c], heap[p]);
            p = c;
        }

        return result;
    }
}
