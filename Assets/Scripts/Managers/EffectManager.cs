using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public static class EffectManager
    {
        // For object with a BC ans State=Ghost, show them green or red based on whether they can be placed
        public static void UpdateGhostVisuals(GameObject go)
        {
            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            if (bc == null)
                return;

            if (bc.State == State.Ghost)
            {
                go.GetComponent<Renderer>().material = Constraint.CheckRootConstraint(go) ? BuildController.instance.GhostGreen : BuildController.instance.GhostRed;
            }
        }

        public static void UpdateBlueprintVisuals(GameObject go)
        {
            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            if (bc == null)
                return;

            if (bc.State == State.Blueprint)
            {
                //TO DO : Should come from BuildingMaterial
                if (bc.BuildingMaterials.Find(bm => bm.Name=="Concrete") != null)
                    go.GetComponent<Renderer>().material = BuildController.instance.Concrete;
                if (bc.BuildingMaterials.Find(bm => bm.Name == "Wood") != null)
                    go.GetComponent<Renderer>().material = BuildController.instance.Wood;
            }
        }
    }
}