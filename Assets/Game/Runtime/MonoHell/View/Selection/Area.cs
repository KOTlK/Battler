using UnityEngine;

namespace Game.Runtime.MonoHell.View.Selection
{
    public struct Area
    {
        public Area(Vector2 position, Vector2 halfExtents)
        {
            Position = position;
            HalfExtents = halfExtents;
            MaxX = Position.x + halfExtents.x;
            MaxY = Position.y + halfExtents.y;
            MinX = Position.x - halfExtents.x;
            MinY = Position.y - halfExtents.y;
        }
        
        public Vector2 Position;
        public Vector2 HalfExtents;
        public float MaxX;
        public float MaxY;
        public float MinX;
        public float MinY;
        
        public Vector2 Size => HalfExtents * 2;

        public bool Contains(Vector2 point)
        {
            return point.x >= MinX &&
                   point.x < MaxX &&
                   point.y >= MinY &&
                   point.y < MaxY;
        }

        public bool Overlaps(Area other)
        {
            return other.MaxX > MinX &&
                   other.MinX < MaxX &&
                   other.MaxY > MinY &&
                   other.MinY < MaxY;
        }
    }
}