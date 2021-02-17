using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class BeyondGroup
    {
        public Vector3 Position { get; protected set; }
        public Quaternion Rotation { get; protected set; }
        public Vector3 RightNormalised { get; protected set; }
        public Vector3 ForwardNormalised { get; protected set; }
        public Vector3 UpNormalised { get; protected set; }
        public string Name { get; protected set; }
        public List<BeyondComponent> ComponentList { get; protected set; }
        public GameObject GroupObject { get; protected set; }
        public BeyondGroup(string s, Vector3 p, Quaternion r)
        {
            Position = p;
            Rotation = r;
            Name = s;
            ComponentList = new List<BeyondComponent>();
            RightNormalised = Utility.RotateAroundPoint(Vector3.right, Vector3.zero, r);
            ForwardNormalised = Utility.RotateAroundPoint(Vector3.forward, Vector3.zero, r);
            UpNormalised = Utility.RotateAroundPoint(Vector3.up, Vector3.zero, r);
            CreateGroupObject();
        }

        public void CreateGroupObject()
        {
            GroupObject = new GameObject();
            GroupObject.name = Name;
            GroupObject.transform.position = Position;
            GroupObject.transform.rotation = Rotation;
        }

        public bool AddBeyondComponent(BeyondComponent bc , Vector3Int localPos)
        {
            // On-demand creation of componentList (for deserialization)
            if (ComponentList == null) ComponentList = new List<BeyondComponent>();

            if (ComponentList.Contains(bc)) return false;
            else
            {
                ComponentList.Add(bc);
                // On-demand creation of group GameObject (for deserialization)
                if (GroupObject == null) CreateGroupObject();

                bc.transform.SetParent(GroupObject.transform, false);
                bc.transform.localPosition = Vector3.zero;
                bc.transform.localRotation = Quaternion.identity;

                return true;
            }
        }

        public bool RemoveBeyondComponent(BeyondComponent bc)
        {
            if (ComponentList.Contains(bc))
            {
                ComponentList.Remove(bc);
                return true;
            }
            return false;
        }

        public List<BeyondComponent> BeyondComponentsAt(Vector3Int p)
        {
            return ComponentList.FindAll(bc => bc.GroupPosition == p);
        }
    }
}