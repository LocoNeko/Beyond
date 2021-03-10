using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class SnapController
    {
        public static float _distanceToSnap = 0.5f;
        public static GameObject SnapCandidate(GameObject go)
        {
            GameObject result = null;
            float dMin = -1;
            foreach(Collider col in Physics.OverlapSphere(go.transform.position , _distanceToSnap))
            {
                if (col.gameObject != go)
                { // Can't collide with self
                    BeyondComponent collidedBC = col.gameObject.GetComponent<BeyondComponent>();
                    if (collidedBC != null)
                    { // Can't collide with an object that has no BC
                        float distance = Vector3.Distance(go.transform.position, col.gameObject.transform.position);
                        if (dMin==-1 || distance < dMin)
                        {
                            dMin = distance;
                            result = col.gameObject;
                        }
                    }
                }
            }
            return result;
        }

        public static GameObject SnapToObject(GameObject ObjectSnapping , out SnapTarget TargetSnapping)
        {
            GameObject result = null;
            TargetSnapping = null;
            float d = -1;
            float dMin = -1;
            // Given an ObjectSnapping, go through all the ObjectSnapping's snaptargets
            // For each snap target, check overlap sphere, return objects that have a centre close enough to that snap target
            // As we go through potential SnapToObjects and corresponding ObjectSnapping targets, always keep the closest only
            BeyondComponent BCSnapping = ObjectSnapping.GetComponent<BeyondComponent>();
            // We want to snap an object that has a Beyond Component
            if (BCSnapping!=null)
            {
                // Go through each SnapTarget of the ObjectSnapping
                foreach (SnapTarget st in BCSnapping.Template.SnapTargets)
                { 
                    Vector3 stCoords = st.GetToCentre(ObjectSnapping);
                    // Go through all objects that overlap with a sphere centered around that SnapTarget
                    foreach (Collider col in Physics.OverlapSphere(stCoords, _distanceToSnap))
                    {
                        // Can't snap to yourself
                        if (col.gameObject != ObjectSnapping)
                        { 
                            BeyondComponent BCSnapped = col.gameObject.GetComponent<BeyondComponent>();
                            // We want to snap to an object that has a BeyondComponent, that has one of the target's ToTags and is in the snaptarget's ToPos
                            if ( (BCSnapped != null) && (BCSnapped.Template.ContainsTag(st.ToTags)) && (BCSnapped.PosInCell == st.ToPos) )
                            {
                                // The snapping object must be in the snaptarget's Posincell
                                if (BCSnapping.PosInCell == st.FromPos)
                                {
                                    // Distance between the snapTarget and the snapped Object's cell centre
                                    d = Vector3.Distance(stCoords , BCSnapped.Template.GetCellCentre(col.gameObject));
                                    if (dMin ==-1 || d<dMin)
                                    {
                                        dMin = d;
                                        result = col.gameObject;
                                        TargetSnapping = st;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}