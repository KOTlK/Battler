using UnityEngine;

namespace Game.Runtime.Extensions
{
    public static class VectorExtensions
    {
        public static Vector2 ToXZ(this Vector3 origin)
        {
            return new Vector2(origin.x, origin.z);
        }

        public static Vector3 FromXZ(this Vector2 origin)
        {
            return new Vector3(origin.x, 0, origin.y);
        }

        public static Vector3 FromXZ(this Vector2 origin, float y)
        {
            return new Vector3(origin.x, y, origin.y);
        }

        public static Vector3 FromXZ(this Vector2Int origin)
        {
            return new Vector3(origin.x, 0, origin.y);
        }
    }
}