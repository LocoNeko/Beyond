using System.Collections;
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

    public class BeyondComponent : MonoBehaviour
    {
        [SerializeField] public Template Template {get; protected set; }
        [SerializeField] public State State { get; protected set; }
        [SerializeField] public List<BuildingMaterial> BuildingMaterials { get; protected set; }
        [SerializeField] public Dictionary<string,bool> CollidingWith { get; protected set; }
        [SerializeField] public Vector3Int GroupPosition { get; protected set; }

        [SerializeField] public string DEBUG_TemplateName;

        public void Start()
        {
            if (BuildingMaterials == null)
                BuildingMaterials = new List<BuildingMaterial>();
            CollidingWith = new Dictionary<string, bool>();
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
            State = s;
            foreach (BuildingMaterial gm in lgm)
            {
                BuildingMaterials.Add(gm);
            }
        }

        public void CopyValues(BeyondComponent bc)
        {
            Template = bc.Template;
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

        public void PlaceGhost()
        {
            if (State == State.Ghost)
            {
                State = State.Blueprint;
            }
        }

        public bool IsCollidingWith(string layerName)
        {
            if (CollidingWith!=null)
                return (CollidingWith.ContainsKey(layerName) && CollidingWith[layerName]);
            return false;
        }

        public void OnTriggerEnter(Collider c)
        {
            string layerName = LayerMask.LayerToName(c.gameObject.layer);
            CollidingWith[layerName] = true;
        }

        public void OnTriggerExit(Collider c)
        {
            string layerName = LayerMask.LayerToName(c.gameObject.layer);
            CollidingWith[layerName] = false;
        }
    }
}