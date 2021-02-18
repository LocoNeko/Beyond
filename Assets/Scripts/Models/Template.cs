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
        //The CellCentre tells us where the cell's centre is compared to the object's centre
        public Vector3 CellCentre { get; protected set; }
        public List<PosInCell> ValidPosInCell { get; protected set; }
        public List<SnapTarget> SnapTargets { get; protected set; }

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
        }

        public Vector3 GetCellCentre(GameObject go)
        {
            return go.transform.position + Utility.RotateAroundPoint(CellCentre, Vector3.zero, go.transform.rotation);
        }

        public void AddSnapTarget(PosInCell fromPos, int x, int y, int z, Template template, PosInCell toPos)
        {
            SnapTarget st = new SnapTarget(fromPos, x, y, z, template, toPos);
            SnapTargets.Add(st);
        }
    }
}