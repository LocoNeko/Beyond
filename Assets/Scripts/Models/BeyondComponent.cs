using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public enum State
    {
        Ghost,
        Blueprint,
        Placed
    }
    public enum PosInCell
    {
        Centre,
        Top,
        Bottom,
        Front,
        Back,
        Left,
        Right
    }

    [Serializable]
    public class BeyondComponent : MonoBehaviour
    {
        [SerializeField] public Template Template {get; protected set; }
        [SerializeField] public State State { get; protected set; }
        [SerializeField] public List<BuildingMaterial> BuildingMaterials { get; protected set; }
        [SerializeField] public PosInCell PosInCell { get; protected set; }
        [SerializeField] public BeyondGroup Group { get; protected set; }
        [SerializeField] public Vector3Int GroupPosition { get; protected set; }

        [SerializeField] public string DEBUG_TemplateName;

        public void Start()
        {
            if (BuildingMaterials == null)
                BuildingMaterials = new List<BuildingMaterial>();
        }

        public void Update()
        {
            DEBUG_TemplateName = (Template == null ? "NULL" : Template.Name);
        }

        public void SetValues(Template t , State s , List<BuildingMaterial> lgm)
        {
            if (BuildingMaterials==null)
                BuildingMaterials = new List<BuildingMaterial>();
            Template = t;
            PosInCell = t.ValidPosInCell[0]; // By default, a BC is in the first PosInCell of its template's ValidPosInCell
            State = s;
            foreach (BuildingMaterial gm in lgm)
            {
                BuildingMaterials.Add(gm);
            }
        }

        public void CopyValues(BeyondComponent bc)
        {
            Template = bc.Template;
            PosInCell = bc.PosInCell;
            State = bc.State;
            if (BuildingMaterials == null)
                BuildingMaterials = new List<BuildingMaterial>();
            foreach (BuildingMaterial gm in bc.BuildingMaterials)
            {
                BuildingMaterials.Add(gm);
            }
        }

        public void SetState(State s)
        {
            State = s;
        }

        public void SetGroupPosition(BeyondGroup bg , Vector3Int pos)
        {
            Group = bg;
            GroupPosition = pos;
        }

        public void PlaceGhost()
        {
            if (State == State.Ghost)
            {
                State = State.Blueprint;
            }
        }
    }
}