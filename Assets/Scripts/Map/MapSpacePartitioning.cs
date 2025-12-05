using System.Collections.Generic;
using UnityEngine;

public static class MapSpacePartitioning
{
    public static List<Room> GenerateRooms(int[] corners, int maxRoomCount, float minRatioXY, float maxRatioXY, int seed = 0)
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
            bool horizontal = Random.value > 0.5f;

            bool canHorizontal = current.yLen > 1;
            bool canVertical = current.xLen > 1;

            if (horizontal)
            {
                int minSplit = current.y + (int)(current.yLen * minRatioXY);
                int maxSplit = current.y + (int)(current.yLen * maxRatioXY);

                if (maxSplit > minSplit)
                {
                    int split = Random.Range(minSplit, maxSplit);

                    Room bottom = new Room(current.x, current.y, current.XMax(), split);
                    Room top = new Room(current.x, split, current.XMax(), current.YMax());

                    queue.Enqueue(bottom);
                    queue.Enqueue(top);
                    continue;
                }
                else if (canVertical)
                {
                    // fallback
                    minSplit = current.x + (int)(current.xLen * minRatioXY);
                    maxSplit = current.x + (int)(current.xLen * maxRatioXY);

                    if (maxSplit > minSplit)
                    {
                        int split = Random.Range(minSplit, maxSplit);

                        Room left = new Room(current.x, current.y, split, current.YMax());
                        Room right = new Room(split, current.y, current.XMax(), current.YMax());

                        queue.Enqueue(left);
                        queue.Enqueue(right);
                        continue;
                    }
                }

                rooms.Add(current);
            }
            else
            {
                int minSplit = current.x + (int)(current.xLen * minRatioXY);
                int maxSplit = current.x + (int)(current.xLen * maxRatioXY);

                if (maxSplit > minSplit)
                {
                    int split = Random.Range(minSplit, maxSplit);

                    Room left = new Room(current.x, current.y, split, current.YMax());
                    Room right = new Room(split, current.y, current.XMax(), current.YMax());

                    queue.Enqueue(left);
                    queue.Enqueue(right);
                    continue;
                }
                else if (canHorizontal)
                {
                    // fallback
                    minSplit = current.y + (int)(current.yLen * minRatioXY);
                    maxSplit = current.y + (int)(current.yLen * maxRatioXY);

                    if (maxSplit > minSplit)
                    {
                        int split = Random.Range(minSplit, maxSplit);

                        Room bottom = new Room(current.x, current.y, current.XMax(), split);
                        Room top = new Room(current.x, split, current.XMax(), current.YMax());

                        queue.Enqueue(bottom);
                        queue.Enqueue(top);
                        continue;
                    }
                }

                rooms.Add(current);
            }
        }
        return rooms;
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
    public int XMax() { return x + xLen; }
    public int YMax() { return y + yLen; }
}
