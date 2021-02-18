using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class SnapController
    {
        public static float distanceToSnap = 0.7f;
        public static GameObject GetSnapCandidate(GameObject go, SnapTarget st)
        {
            GameObject candidate = null;
            float minD = -1;
            Vector3 snapFrom = st.GetToCentre(go.transform);
            foreach (Collider col in Physics.OverlapSphere(snapFrom, 3f))
            {
                BeyondComponent bc = col.gameObject.GetComponent<BeyondComponent>();
                if (bc!=null)
                {
                    float d = Vector3.Distance(snapFrom, bc.Template.GetCellCentre(col.gameObject));
                    if ( (d<=distanceToSnap) && (col.gameObject!=go) && (bc.Template == st.ToTemplate) && (bc.PosInCell == st.ToPos))
                    {
                        // We found a candidate !
                        if (d<minD || minD== -1)
                        {
                            minD = d;
                            candidate = col.gameObject;
                        }
                    }
                }
            }
            return candidate;
        }

        // Get the closest snap Candidate to the object
        public static GameObject GetSnapCandidate(GameObject go)
        {
            GameObject candidate = null;
            float minD = -1;
            Template t = go.GetComponent<BeyondComponent>().Template;
            foreach (SnapTarget st in t.SnapTargets)
            {
                GameObject snapTarget = GetSnapCandidate(go, st);
                if (snapTarget != null)
                {
                    float d = Vector3.Distance(go.transform.position, snapTarget.transform.position);
                    if (d < minD || minD == -1)
                    {
                        minD = d;
                        candidate = snapTarget;
                    }
                }
            }
            return candidate;
        }


        // Here in case I ever need to see all Snap Candidates of an object. Requires a list of test spheres (or whatever) to visually show them
        public static void DebugSnap(List<GameObject> testSpheres , GameObject blueprint)
        {
            foreach (GameObject go in testSpheres)
                go.transform.position = Vector3.zero;
            int i = 0;
            Template t = blueprint.GetComponent<BeyondComponent>().Template;
            foreach (SnapTarget st in t.SnapTargets)
            {
                GameObject snapTarget = SnapController.GetSnapCandidate(blueprint, st);
                if (snapTarget != null)
                    testSpheres[i++].transform.position = snapTarget.transform.position;
            }

        }
    }
}