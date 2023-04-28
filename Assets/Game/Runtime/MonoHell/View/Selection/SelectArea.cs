using UnityEngine;

namespace Game.Runtime.MonoHell.View.Selection
{
    public struct SelectArea
    {
        public Vector2 Position;
        public Vector2 HalfExtents;
        public Vector2 Size => HalfExtents * 2;

        public bool Contains(Vector2 point)
        {
            return point.x >= Position.x - HalfExtents.x &&
                   point.x < Position.x + HalfExtents.x &&
                   point.y >= Position.y - HalfExtents.y &&
                   point.y < Position.y + HalfExtents.y;
        }
    }
}