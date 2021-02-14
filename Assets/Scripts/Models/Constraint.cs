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
        TopClear
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

        public static bool IsWellFormed(Constraint c)
        {
            if (c.Operation == Operation.And || c.Operation == Operation.Or)
            {
                if (c.Operands.Count < 2)
                {
                    Debug.LogError("Constraint And/Or expects at least 2 operands");
                    return false;
                }

                foreach (object o in c.Operands)
                {
                    if (o.GetType() != typeof(Constraint))
                    {
                        Debug.LogError("Constraint And/Or expects all operands to be Constraints");
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


            return true;
        }

        public static bool CheckConstraint(GameObject go , Constraint c = null)
        {
            if (!IsWellFormed(go))
                return false;

            if (c==null)
                c = go.GetComponent<BeyondComponent>().Template.Constraint;

            switch (c.Operation)
            {
                case (Operation.And):
                    return CheckAnd(go, c);
                case (Operation.BaseIn):
                    return CheckBaseIn(go , c);
                case (Operation.TopClear):
                    return CheckTopClear(go, c);
                default:
                    break;
            }
            return false;
        }

        public static bool CheckAnd(GameObject go, Constraint c)
        {
            if (!IsWellFormed(go))
                return false;

            if (!IsWellFormed(c))
                return false;

            foreach (object o in c.Operands)
            {
                if (!CheckConstraint(go, (Constraint)o))
                    return false;
            }

            return true;
        }
        public static bool CheckBaseIn(GameObject go , Constraint c)
        {
            if (!IsWellFormed(go))
                return false;

            if (!IsWellFormed(c))
                return false;

            float depth = (float)c.Operands[0] ;

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
    
        public static bool CheckTopClear(GameObject go , Constraint c)
        {
            if (!IsWellFormed(go))
                return false;

            Template template = go.GetComponent<BeyondComponent>().Template;
            Vector3 castFrom = go.transform.position;
            castFrom.y = 1000f; // TO DO Cast from the highest possible altitude
            float rayLength = castFrom.y - go.transform.position.y - template.CastBox.y * 2;
            bool result = Physics.BoxCast(castFrom, template.CastBox, Vector3.down, go.transform.rotation, rayLength, LayerMask.GetMask("Terrain"));
            return !result;
        }
    }
}