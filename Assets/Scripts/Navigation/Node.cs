using UnityEngine;

namespace Game.Navigation
{
    [System.Serializable]
    public class Node
    {
        public int gridX;
        public int gridY;
        public MovementType allowedMovement;
        public float gCost;
        public float hCost;
        public Node parent;

        public float fCost => gCost + hCost;

        public Node(int x, int y, MovementType movement)
        {
            gridX = x;
            gridY = y;
            allowedMovement = movement;
        }
    }
}
