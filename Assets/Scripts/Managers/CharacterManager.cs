using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Beyond
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private GameObject FPSController;
        void Update()
        {
            Terrain closestTerrain = GetClosestCurrentTerrain(FPSController.transform.position);
            Vector3 terrainlocalPosition = FPSController.transform.position - closestTerrain.transform.position;
            Vector2 normalisedPosition = new Vector2(Mathf.InverseLerp(0f, closestTerrain.terrainData.size.x, terrainlocalPosition.x)* closestTerrain.terrainData.size.x,
                                                    Mathf.InverseLerp(0f, closestTerrain.terrainData.size.z, terrainlocalPosition.z)* closestTerrain.terrainData.size.z);
            //Debug.Log("Closest terrain is " + closestTerrain.name + " position is : " + normalisedPosition);
        }

        Terrain GetClosestCurrentTerrain(Vector3 playerPos)
        {
            //Get all terrain
            Terrain[] terrains = Terrain.activeTerrains;

            //Make sure that terrains length is ok
            if (terrains.Length == 0)
                return null;

            //If just one, return that one terrain
            if (terrains.Length == 1)
                return terrains[0];

            //Get the closest one to the player
            float lowDist = (terrains[0].GetPosition() - playerPos).sqrMagnitude;
            var terrainIndex = 0;

            for (int i = 1; i < terrains.Length; i++)
            {
                Terrain terrain = terrains[i];
                Vector3 terrainPos = terrain.GetPosition();

                //Find the distance and check if it is lower than the last one then store it
                var dist = (terrainPos - playerPos).sqrMagnitude;
                if (dist < lowDist)
                {
                    lowDist = dist;
                    terrainIndex = i;
                }
            }
            return terrains[terrainIndex];
        }
    }

}