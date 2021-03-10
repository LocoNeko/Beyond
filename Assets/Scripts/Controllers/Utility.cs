using System;
using UnityEngine;
using System.Collections.Generic;

namespace Beyond
{
    [Serializable]
    public class BeyondException : Exception
    {
        public BeyondException() { }

        public BeyondException(string msg) : base("Beyond Exception: " + msg)
        { }
    }

    public static class Utility
    {
        public static Vector3 RotateAroundPoint(Vector3 p, Vector3 pivot, Quaternion r)
        {
            Vector3 result = r * (p - pivot) + pivot;
            //Debug.Log(string.Format("RotateAroundPoint {0} changed to {1}",p, result));
            return result;
        }

        // Rotates the 3 unit axes (right,up,forward) by rotation r around origin
        public static Dictionary<string,Vector3> RotatedAxes(Quaternion r)
        {
            Dictionary<string, Vector3> result = new Dictionary<string, Vector3>();
            result["x"] = Utility.RotateAroundPoint(Vector3.right, Vector3.zero, r);
            result["y"] = Utility.RotateAroundPoint(Vector3.up, Vector3.zero, r);
            result["z"] = Utility.RotateAroundPoint(Vector3.forward, Vector3.zero, r);
            return result;
        }

        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
        {
            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);

            if (dotDenominator != 0.0f)
            {
                length = dotNumerator / dotDenominator;

                vector = lineVec * length ;

                intersection = linePoint + vector;

                return true;
            }

            else
                return false;
        }
    }
}