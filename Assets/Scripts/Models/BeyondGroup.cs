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

        private GameObject CreateGroupObject()
        {
            GroupObject = new GameObject();
            GroupObject.name = Name;
            GroupObject.transform.position = Position;
            GroupObject.transform.rotation = Rotation;
            return GroupObject;
        }

        public void AddObject(GameObject go)
        {
            if (go == null)
                throw new BeyondException("GameObject is null");

            if (GroupObject==null)
                throw new BeyondException("GroupObject is null");

            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            if (bc == null)
                throw new BeyondException("No Beyond Component attached to GameObject");

            // Set BC's group position
            Vector3Int groupPosition = Vector3Int.RoundToInt(Utility.RotateAroundPoint(go.transform.position - Position, Vector3.zero, Rotation));

            //TO DO : check if this position is already occupied, which is tricky since some objects (like walls or cables) can co-exist in the same cell
            bc.SetGroupPosition(groupPosition);
            go.transform.SetParent(GroupObject.transform);
            ComponentList.Add(bc);
        }



        public List<BeyondComponent> BeyondComponentsAt(Vector3Int p)
        {
            return ComponentList.FindAll(bc => bc.GroupPosition == p);
        }
    }
}