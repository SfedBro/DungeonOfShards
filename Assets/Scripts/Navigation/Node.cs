using UnityEngine;

namespace Game.Navigation
{
    [System.Serializable]
    public class Node
    {
        public int gridX;
        public int gridY;
        public bool free; // whether not occupied by anything movable
        public MovementType allowedMovement;
        public float gCost;
        public float hCost;
        public Node parent;

        public float fCost => gCost + hCost;

        public Node(int x, int y, bool free, MovementType movement)
        {
            gridX = x;
            gridY = y;
            this.free = free;
            allowedMovement = movement;
        }
    }
}
