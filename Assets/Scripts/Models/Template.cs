using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{


    public class Template
    {
        public string Name{ get; protected set; }
        public GameObject Prefab { get; protected set; }
        public Constraint Constraint{ get; protected set; }

        // The cast box allows us to prevent the object from being inside terrain and is a good alternative to colliders
        public Vector3 CastBox { get; protected set; }
        public List<PosInCell> ValidPosInCell { get; protected set; }
        //The CellCentre tells us where the cell's centre is compared to the object's centre
        public Vector3 CellCentre { get; protected set; }
        public List<SnapTarget> SnapTargets { get; protected set; }
        public List<string> Tags { get; protected set; }

        public Template(string name, GameObject prefab, Constraint constraint , Vector3 castBox , List<PosInCell> validPosInCell , Vector3? cellCentre = null)
        {
            Name = name ;
            Prefab = prefab ;
            Constraint = constraint;
            CastBox = castBox;
            CellCentre = ( (cellCentre != null) ? (Vector3)cellCentre : Vector3.zero) ;
            ValidPosInCell = new List<PosInCell>();
            foreach (PosInCell pc in validPosInCell)
            {
                ValidPosInCell.Add(pc);
            }
            SnapTargets = new List<SnapTarget>();
            Tags = new List<string>();
        }

        public void SetValues(Template t)
        {
            Name = t.Name;
            Prefab = t.Prefab;
            Constraint = t.Constraint;
            CastBox = t.CastBox;
            CellCentre = t.CellCentre;
            foreach (PosInCell pc in t.ValidPosInCell)
            {
                ValidPosInCell.Add(pc);
            }
            foreach (SnapTarget st in t.SnapTargets)
            {
                SnapTargets.Add(st);
            }
            foreach (string s in t.Tags)
            {
                Tags.Add(s);
            }
        }

        // World space coordinates of this GameObject's CellCentre, based on its world space centre and its template
        public Vector3 GetCellCentre(GameObject go)
        {
            return go.transform.position + Utility.RotateAroundPoint(CellCentre, Vector3.zero, go.transform.rotation);
        }

        public void AddSnapTarget(PosInCell fromPos, int x, int y, int z, List<string> totags, PosInCell toPos)
        {
            SnapTarget st = new SnapTarget(fromPos, x, y, z, totags, toPos);
            SnapTargets.Add(st);
        }

        public void AddTag(string s)
        {
            if (Tags==null)
                Tags = new List<string>();
            Tags.Add(s);
        }

        public bool ContainsTag(string s)
        {
            return Tags.Contains(s);
        }
        public bool ContainsTag(List<string> ls)
        {
            foreach (string s in ls)
            {
                if (Tags.Contains(s))
                    return true;
            }
            return false;
        }
    }
}