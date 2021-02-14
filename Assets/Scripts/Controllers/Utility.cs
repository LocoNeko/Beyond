using UnityEngine;

namespace Beyond
{
    public static class Utility
    {
        public static Vector3 RotateAroundPoint(Vector3 p, Vector3 pivot, Quaternion r)
        {
            return r * (p - pivot) + pivot;
        }
    }
}