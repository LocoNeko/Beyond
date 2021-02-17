using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond
{
    public class BuildingMaterial
    {
        public string Name { get; protected set; }

        // TO DO : eventually, I may directly use BuildingMaterials in the ActionController to avoid this ugly casting
        public static List<BuildingMaterial> ActionsToMaterials(List<GameAction> materialsAsActions)
        {
            List<BuildingMaterial> result = new List<BuildingMaterial>();
            foreach (GameAction ga in materialsAsActions)
            {
                result.Add(GameManager.instance.BuildingMaterials.Find(gm => gm.Name == ga.Name));
            }
            return result;
        }

        public BuildingMaterial(string s)
        {
            Name = s;
        }
    }
}