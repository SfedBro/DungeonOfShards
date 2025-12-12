using System.Collections.Generic;
using UnityEngine;

public static class MapSpacePartitioning
{
    public static List<Room> GenerateRooms(int[] corners, int maxRoomCount, float minRatioXY, float maxRatioXY, int minRoomSize = 1, bool useOppositeAxis = true, int seed = 0)
    {
        Random.InitState(seed);
        List<Room> rooms = new List<Room>();
        Queue<Room> queue = new Queue<Room>();
        // [xMin, yMin, xMax, yMax]
        Room root = new Room(corners[0], corners[1], corners[2], corners[3]);
        queue.Enqueue(root);
        while (queue.Count > 0 && rooms.Count + queue.Count < maxRoomCount)
        {
            Room current = queue.Dequeue();
            bool canSplitHorizontal = current.yLen >= minRoomSize * 2;
            bool canSplitVertical = current.xLen >= minRoomSize * 2;
            bool horizontal = Random.value > 0.5f;

            bool attempted = false;
            bool success = false;

            if (horizontal && canSplitHorizontal)
            {
                success = TrySplitHorizontal(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);
                attempted = true;
            }
            else if (!horizontal && canSplitVertical)
            {
                success = TrySplitVertical(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);
                attempted = true;
            }

            // 2) did not succeed so attempting otherwise
            if (!success)
            {
                if (!attempted)  // did not attempt due the conditions
                {
                    if (!horizontal && canSplitHorizontal)
                        success = TrySplitHorizontal(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);

                    else if (horizontal && canSplitVertical)
                        success = TrySplitVertical(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);
                }
                else // attempted unsuccessfull
                {
                    if (horizontal && canSplitVertical)
                        success = TrySplitVertical(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);

                    else if (!horizontal && canSplitHorizontal)
                        success = TrySplitHorizontal(current, queue, minRoomSize, minRatioXY, maxRatioXY, useOppositeAxis);
                }
            }

            // 3) room cannot be split
            if (!success)
            {
                rooms.Add(current);
            }
        }
        while (queue.Count > 0)
        {
            rooms.Add(queue.Dequeue());
        }
        return rooms;
    }
    static bool TrySplitHorizontal(Room r, Queue<Room> q, int minRoomSize, float minRatio, float maxRatio, bool useOppositeAxis)
    {
        //Debug.Log("Horizontal");
        int axisLen = useOppositeAxis ? r.xLen : r.yLen;
        int relMin = Mathf.CeilToInt(axisLen * minRatio);
        int relMax = Mathf.FloorToInt(axisLen * maxRatio);

        int minSplit = Mathf.Max(r.y + relMin, r.y + minRoomSize);
        int maxSplit = Mathf.Min(r.YMax() - minRoomSize, r.y + relMax);

        if (maxSplit < minSplit) return false;

        int split = Random.Range(minSplit, maxSplit + 1);

        //Debug.Log($"{r.y}, {split}, {r.YMax()}");

        Room bottom = new Room(r.x, r.y, r.XMax(), split);
        Room top = new Room(r.x, split, r.XMax(), r.YMax());
        q.Enqueue(bottom);
        q.Enqueue(top);
        return true;
    }
    static bool TrySplitVertical(Room r, Queue<Room> q, int minRoomSize, float minRatio, float maxRatio, bool useOppositeAxis)
    {
        //Debug.Log("Vertical");
        int axisLen = useOppositeAxis ? r.yLen : r.xLen;
        int relMin = Mathf.CeilToInt(axisLen * minRatio);
        int relMax = Mathf.FloorToInt(axisLen * maxRatio);

        int minSplit = Mathf.Max(r.x + relMin, r.x + minRoomSize);
        int maxSplit = Mathf.Min(r.XMax() - minRoomSize, r.x + relMax);

        if (maxSplit < minSplit) return false;

        int split = Random.Range(minSplit, maxSplit + 1);

        //Debug.Log($"{r.x}, {split}, {r.XMax()}");

        Room left = new Room(r.x, r.y, split, r.YMax());
        Room right = new Room(split, r.y, r.XMax(), r.YMax());
        q.Enqueue(left);
        q.Enqueue(right);
        return true;
    }
}

public struct Room
{
    public int x, y, xLen, yLen;
    public Room(int ix, int iy, int ax, int ay) // mIn x,y; mAx x, y
    {
        x = ix;
        y = iy;
        xLen = ax - ix;
        yLen = ay - iy;
    }
    public readonly int XMax() { return x + xLen; }
    public readonly int YMax() { return y + yLen; }
}
