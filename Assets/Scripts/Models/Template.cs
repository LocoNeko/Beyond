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
        //The pivotOffset tells us where the cell's centre is compared to the object's centre
        public Vector3 PivotOffset { get; protected set; }

        public Template(string name, GameObject prefab, Constraint constraint , Vector3 castBox , Vector3? pivotOffset = null)
        {
            Name = name ;
            Prefab = prefab ;
            Constraint = constraint;
            CastBox = castBox;
            PivotOffset = ( (pivotOffset != null) ? (Vector3)pivotOffset : Vector3.zero) ;
        }

        public void SetValues(Template t)
        {
            Name = t.Name;
            Prefab = t.Prefab;
            Constraint = t.Constraint;
            CastBox = t.CastBox;
            PivotOffset = t.PivotOffset;
        }
    }
}