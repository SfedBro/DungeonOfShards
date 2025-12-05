using System;
using System.Collections.Generic;
using UnityEngine;

public static class DjeikstraCorridorMaps
{
    public static int[,] MakeCorridors(List<Room> rooms, int corridorWidth, int seed = 0)
    {
        UnityEngine.Random.InitState(seed);
        List<Tuple<int, int>> roomPositions = new List<Tuple<int, int>>();
        foreach (Room r in rooms)
        {
            Tuple<int, int> curPos = new Tuple<int, int> (r.XMax() - r.x / 2, r.YMax() - r.y / 2);
            roomPositions.Add(curPos);
        }
        return new int[1,1];
    }
}
