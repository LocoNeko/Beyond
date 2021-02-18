using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class SnapTarget
    {
        public PosInCell FromPos { get; protected set; }
        public Vector3Int ToCentre { get; protected set; }
        public Template ToTemplate { get; protected set; }
        public PosInCell ToPos { get; protected set; }

        public SnapTarget (PosInCell p1, Vector3Int v, Template t, PosInCell p2)
        {
            /*
             * SnapTargets allow objects to snap to other based on their template
             * FromPos = the PosInCell of the object is its cell, based on its template and sometimes rotation
             * ToCentre = the object can snap to a target object with its cell centre located at its own cell centre + ToCentre
             * ToPos = the target object must be at that PosInCell in its cell
             */
            FromPos = p1;
            ToCentre = v;
            ToTemplate = t;
            ToPos = p2;
        }

        public SnapTarget(PosInCell fromPos, int x, int y, int z, Template template, PosInCell toPos) : this(fromPos, Vector3Int.zero , template, toPos)
        {
            ToCentre = new Vector3Int(x, y, z);
        }

        // World coordinates of the centre for this SnapTarget
        public Vector3 GetToCentre(Transform tr)
        {
            return tr.position + Utility.RotateAroundPoint(ToCentre, Vector3.zero, tr.rotation);
        }
    }
}