using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public enum Operation
    {
        Or,
        And,
        BaseIn,
        TopClear,
        AvoidCollision
    }
    public class Constraint
    {
        public Operation Operation { get; protected set; }
        public List<System.Object> Operands { get; protected set; }

        public Constraint (Operation operation , List<System.Object> operands)
        {
            Operation = operation;
            Operands = operands;
        }

        public static bool IsWellFormed(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("Attempting to check constraints on null");
                return false;
            }

            if (go.GetComponent<BeyondComponent>() == null)
            {
                Debug.LogError(String.Format("Attempting to check constraints on object {0} which doesn't have a BeyondComponent", go.name));
                return false;
            }

            if (go.GetComponent<BeyondComponent>().Template == null)
            {
                Debug.LogError(String.Format("Attempting to check constraints on object {0} which doesn't have a Template", go.name));
                return false;
            }

            return true;
        }

        public static Constraint GetGameObjectConstraint(GameObject go)
        {
            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            if (bc == null)
            {
                Debug.LogError("Attempting to get constraints on an Object without a BeyondComponent");
                return null;
            }
            Template t = bc.Template;
            if (t == null)
            {
                Debug.LogError("Attempting to get constraints on an Object without a Template in its BeyondComponent");
                return null;
            }
            return t.Constraint;
        }

        public static bool IsWellFormed(Constraint c)
        {
            if (c.Operation == Operation.And || c.Operation == Operation.Or)
            {
                if (c.Operands.Count < 2)
                {
                    Debug.LogError("Constraint "+ c.Operation.ToString() +" expects at least 2 operands");
                    return false;
                }

                foreach (object o in c.Operands)
                {
                    if (o.GetType() != typeof(Constraint))
                    {
                        Debug.LogError("Constraint " + c.Operation.ToString() + " expects all operands to be Constraints");
                        return false;
                    }
                }
            }

            if (c.Operation == Operation.BaseIn)
            {
                if ((c.Operands.Count != 1) || (c.Operands[0].GetType() != typeof(float)))
                {
                    Debug.LogError("Constraint BaseIn expects only one operand of type float");
                    return false;
                }
            }

            if (c.Operation == Operation.TopClear)
            {
                if (c.Operands.Count != 0)
                {
                    Debug.LogError("Constraint TopClear expects no operand");
                    return false;
                }
            }

            if (c.Operation == Operation.AvoidCollision)
            {
                if ((c.Operands.Count) <1)
                {
                    Debug.LogError("Constraint AvoidCollision expects at least one layer name");
                    return false;
                }

                foreach (object o in c.Operands)
                {
                    if (o.GetType() != typeof(string) || LayerMask.NameToLayer((string)o) == -1)
                    {
                        Debug.LogError("Constraint AvoidCollision expects all operands to be an existing layer name");
                        return false;
                    }
                }

            }

            return true;
        }

        public static bool CheckRootConstraint(GameObject go)
        {
            Constraint c = GetGameObjectConstraint(go);
            return (c==null ? false : c.CheckConstraint(go));
        }

        public bool CheckConstraint(GameObject go)
        {
            if (!IsWellFormed(go))
                return false;

            if (!IsWellFormed(this))
                return false;

            switch (Operation)
            {
                case (Operation.Or):
                    return CheckOr(go);
                case (Operation.And):
                    return CheckAnd(go);
                case (Operation.BaseIn):
                    return CheckBaseIn(go);
                case (Operation.TopClear):
                    return CheckTopClear(go);
                case Operation.AvoidCollision:
                    return CheckAvoidCollision(go);
                default:
                    break;
            }
            return false;
        }

        public bool CheckOr(GameObject go)
        {
            foreach (object o in Operands)
            {
                Constraint c = (Constraint)o;
                if (c.CheckConstraint(go))
                    return true;
            }

            return false;
        }
        public bool CheckAnd(GameObject go)
        {
            foreach (object o in Operands)
            {
                Constraint c = (Constraint)o;
                if (!c.CheckConstraint(go))
                    return false;
            }

            return true;
        }

        public bool CheckBaseIn(GameObject go)
        {
            float depth = (float)Operands[0] ;

            Vector3 BoxCast = go.GetComponent<BeyondComponent>().Template.CastBox;
            Vector3 p = go.transform.position;
            Quaternion q = go.transform.rotation;

            // Cast 4 boxes at each corner of the bottom of the object
            // Their centres are: p + BoxCast in both X and Z, plus FoundationInTerrainBy / 2
            for (float i = -1; i <= 1; i += 2)
            {
                for (float j = -1; j <= 1; j += 2)
                {
                    Vector3 point = (p + new Vector3((BoxCast.x - depth / 2) * i, -BoxCast.y + depth , (BoxCast.z - depth / 2) * j));
                    point = Utility.RotateAroundPoint(point, p, q);

                    //Debug.DrawLine(point , new Vector3(point.x , point.y - 2f , point.z) , Color.red , 0.5f);
                    // The height of the boxes' starting point must be offset by the template's half height (BoxCast.y)
                    if (Physics.BoxCast(point - (Vector3.down * BoxCast.y) , BoxCast, Vector3.down, q, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    
        public bool CheckTopClear(GameObject go)
        {
            Template template = go.GetComponent<BeyondComponent>().Template;
            Vector3 castFrom = go.transform.position;
            castFrom.y = 1000f; // TO DO Cast from the highest possible altitude
            float rayLength = castFrom.y - go.transform.position.y - template.CastBox.y * 2;
            bool result = Physics.BoxCast(castFrom, template.CastBox, Vector3.down, go.transform.rotation, rayLength, LayerMask.GetMask("Terrain"));
            return !result;
        }

        public bool CheckAvoidCollision(GameObject go)
        {
            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            foreach (string layerName in Operands)
            {
                if (bc.IsCollidingWith(layerName))
                    return false;
            }
            return true;
        }
    }
}