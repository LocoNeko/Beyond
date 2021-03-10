using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class SnapTarget
    {
        public PosInCell FromPos { get; protected set; }
        public Vector3Int ToCentre { get; protected set; }
        public List<string> ToTags { get; protected set; }
        public PosInCell ToPos { get; protected set; }

        public SnapTarget (PosInCell p1, Vector3Int v, List<string> totags, PosInCell p2)
        {
            /*
             * SnapTargets allow objects to snap to other based on their template
             * FromPos = the PosInCell of the object is its cell, based on its template and sometimes rotation
             * ToCentre = the object can snap to a target object with its cell centre located at its own cell centre + ToCentre
             * ToTags = the target's template must contain one of those tags
             * ToPos = the target object must be at that PosInCell in its cell
             */
            FromPos = p1;
            ToCentre = v;
            ToTags = new List<string>();
            foreach (string s in totags)
            {
                ToTags.Add(s);
            }
            ToPos = p2;
        }

        public SnapTarget(PosInCell fromPos, int x, int y, int z, List<string> totags, PosInCell toPos) : this(fromPos, Vector3Int.zero , totags, toPos)
        {
            ToCentre = new Vector3Int(x, y, z);
        }

        // World coordinates of the centre for this SnapTarget
        public Vector3 GetToCentre(GameObject go)
        {
            Vector3 result = go.GetComponent<BeyondComponent>().Template.GetCellCentre(go) + 
                    Utility.RotateAroundPoint(ToCentre, Vector3.zero, go.transform.rotation);
            return result;
        }

    }
}