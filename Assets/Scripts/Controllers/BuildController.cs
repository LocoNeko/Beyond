using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beyond
{
    public class BuildController : MonoBehaviour
    {
        public static float blueprintDistanceFromCamera = 5f;
        public static BuildController instance;
        public static int MaxDraggedObjects = 200;
        public List<GameObject> TestSpheres;

        [SerializeField] private GameObject FPSCharacter;
        [SerializeField] public GameObject ActiveBlueprint { get; protected set; }
        [SerializeField] public Material GhostGreen;
        [SerializeField] public Material GhostRed;
        [SerializeField] public Material Concrete;

        // Dragging stuff
        [SerializeField] public GameObject GameObject_DragFrom { get; protected set; }
        private List<GameObject> draggedObjects;
        [SerializeField] private Vector3Int positionInt_DragTo;
        [SerializeField] private Dictionary<string, Vector3> dragDirections;

        // Snapping Stuff
        [SerializeField] private static float SnapCheckFrequency = 0.1f;
        private float timer;

        void Start()
        {
            draggedObjects = new List<GameObject>();
            dragDirections = new Dictionary<string, Vector3>();
            instance = this;
        }

        void Update()
        {
            // We are dragging
            if (GameObject_DragFrom != null)
            {
                ShowDraggedObjects();
            }

            // If we have an active blueprint, update its visuals (keep it horizontal, green/red based on whehter it can be placed, etc)
            else if (ActiveBlueprint != null)
            {
                timer += Time.deltaTime;
                if (timer>=SnapCheckFrequency)
                {
                    timer = 0f;
                    ActiveBlueprint.transform.localRotation = Quaternion.Inverse(FPSCharacter.transform.localRotation);
                    EffectManager.UpdateGhostVisuals(ActiveBlueprint);
                    GameObject ClosestSnapCandidate = SnapController.GetSnapCandidate(ActiveBlueprint);
                    TestSpheres[0].transform.position = ((ClosestSnapCandidate != null) ? ClosestSnapCandidate.transform.position : Vector3.zero);
                }
            }
        }

        public void CreateBlueprint(GameAction blueprint, List<BuildingMaterial> materials)
        {
            // Instantiate a prefab of that blueprint
            // Show it floating 5 units away from where the FPS is looking
            Template t = GameManager.instance.GetTemplate(blueprint.Name);

            if (t !=null)
            {
                DestroyActiveBlueprint();
                ActiveBlueprint = Instantiate(t.Prefab, FPSCharacter.transform, false);
                BeyondComponent bc = ActiveBlueprint.AddComponent<BeyondComponent>();
                bc.SetValues(t , State.Ghost , materials);
                ActiveBlueprint.transform.Translate(new Vector3(0, 0, blueprintDistanceFromCamera));
                ActiveBlueprint.name = string.Format("Ghost ({0})" , t.Name);
            }
            else
            {
                Debug.LogError("Attempting to create null template");
            }
        }

        /*
         * SUPERCEEDED by dragging (or is it ?)
        public void TryPlacingBlueprint()
        {
            //TO DO : SHould check if we are on UI, don't click if we are
            if (ActiveBlueprint!=null)
            {
                if ( Constraint.CheckRootConstraint(ActiveBlueprint))
                {
                    GameObject PlacedObject = Instantiate(ActiveBlueprint , ActiveBlueprint.transform.position , ActiveBlueprint.transform.rotation);

                    // Copy BeyondComponent values from the blueprint to the PlacedObject
                    BeyondComponent blueprintBC = ActiveBlueprint.GetComponent<BeyondComponent>();
                    BeyondComponent bc = PlacedObject.GetComponent<BeyondComponent>();
                    bc.SetValues(blueprintBC.Template, State.Ghost , blueprintBC.BuildingMaterials);

                    // Remove Outline, disable "isTrigger", set layer, name object
                    PlacedObject.GetComponent<BoxCollider>().isTrigger = false;
                    PlacedObject.layer = LayerMask.NameToLayer("Buildings");
                    PlacedObject.name = bc.Template.Name;
                    CreateNewBeyondGroup(bc);
                }
                DestroyActiveBlueprint();
            }
        }
        */

        public void StartDragging()
        {
            if (ActiveBlueprint != null)
            {
                if (Constraint.CheckRootConstraint(ActiveBlueprint))
                {
                    GameObject_DragFrom = Instantiate(ActiveBlueprint, ActiveBlueprint.transform.position, ActiveBlueprint.transform.rotation);
                    BeyondComponent bc = ActiveBlueprint.GetComponent<BeyondComponent>();
                    GameObject_DragFrom.GetComponent<BeyondComponent>().CopyValues(bc);

                    dragDirections = Utility.RotatedAxes(GameObject_DragFrom.transform.rotation);

                    // Clearing & initialising the pool of DraggedObjects
                    for (int i=0; i<MaxDraggedObjects; i++)
                    {
                        GameObject go = Instantiate(ActiveBlueprint);
                        go.SetActive(false);
                        // I need to initialise the BC of each dragged object so I know what template they are. Instantiating them was not enough to copy the ActiveBlueprint's BC
                        go.GetComponent<BeyondComponent>().CopyValues(bc);
                        draggedObjects.Add(go);
                    }

                    ActiveBlueprint.SetActive(false);
                }
            }
            Debug.Log("Start Dragging initialised draggedObjects: "+draggedObjects.Count);
        }

        public void StopDragging()
        {
            List<GameObject> objectsPlaced = new List<GameObject>();
            GameObject FirstObject = Instantiate(GameObject_DragFrom);
            BeyondComponent BC_DragFrom = GameObject_DragFrom.GetComponent<BeyondComponent>();
            FirstObject.GetComponent<BeyondComponent>().CopyValues(BC_DragFrom);
            SetBlueprintFromGhost(FirstObject);
            objectsPlaced.Add(FirstObject);
            Destroy(GameObject_DragFrom);

            foreach (GameObject go in draggedObjects)
            {
                // TO DO : We need to check a bit more than that: can't have a bunch of objects split in several groups because they failed constraint. All draggedObject should form 1 block
                if (Constraint.CheckRootConstraint(go))
                {
                    GameObject ThisObject = Instantiate(go);
                    BeyondComponent BC_ThisObject = go.GetComponent<BeyondComponent>();
                    ThisObject.GetComponent<BeyondComponent>().CopyValues(BC_ThisObject);
                    SetBlueprintFromGhost(ThisObject);
                    objectsPlaced.Add(ThisObject);
                }
                Destroy(go);
            }
            draggedObjects.Clear();
            DestroyActiveBlueprint();

            // Create BeyondGroup, add all objects to it
            BeyondGroup bg = null;
            foreach (GameObject go in objectsPlaced)
            {
                if (bg == null)
                    bg = CreateNewBeyondGroup(go);
                bg.AddObject(go);
                // Only set the layer at the end, so objects don't collide into each other and fail the constraint check
                go.layer = LayerMask.NameToLayer("Buildings");
            }
        }

        void SetBlueprintFromGhost(GameObject go)
        {
            go.GetComponent<BoxCollider>().isTrigger = false;
            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            go.name = bc.Template.Name;
            bc.SetState(State.Blueprint);
            EffectManager.UpdateBlueprintVisuals(go);
        }

        public void ShowDraggedObjects()
        {
            // Find the "to" location based on reference object and camera position and whére I'm löokìng
            Vector3 pos;
            if (Utility.LinePlaneIntersection(out pos, FPSCharacter.transform.position, FPSCharacter.transform.forward, Vector3.up, GameObject_DragFrom.transform.position))
            {
                // Find the corresponding position in units of 1
                Vector3 Diff = pos - GameObject_DragFrom.transform.position; // the Vecotr3 difference between where we are dragging from and to
                Vector3 DiffRotated = Utility.RotateAroundPoint(Diff, Vector3.zero , Quaternion.Inverse(GameObject_DragFrom.transform.rotation)); // Counter-rotate it to cancel the rotation of the object we're dragging
                Vector3Int newPositionInt_DragTo = Vector3Int.RoundToInt(DiffRotated); // round it to integers

                // Only update if we are dragging to a new position
                if (newPositionInt_DragTo != positionInt_DragTo)
                {
                    positionInt_DragTo = newPositionInt_DragTo;
                    // Compare to my current dragged objects positions
                    // Create/Enable all pooled Objects that are in there, put them where they should be thanks to the dragDirections we set when we started dragging

                    int i = 0;
                    for (int z = 0; Mathf.Abs(z) <= Mathf.Abs(positionInt_DragTo.z); z += (positionInt_DragTo.z >= 0 ? 1 : -1))
                    {
                        for (int y = 0; Mathf.Abs(y) <= Mathf.Abs(positionInt_DragTo.y); y += (positionInt_DragTo.y >= 0 ? 1 : -1))
                        {
                            for (int x = 0; Mathf.Abs(x) <= Mathf.Abs(positionInt_DragTo.x); x += (positionInt_DragTo.x >= 0 ? 1 : -1))
                            {
                                if (x != 0 || y != 0 || z != 0)
                                {
                                    draggedObjects[i].transform.position = GameObject_DragFrom.transform.position + x * dragDirections["x"] + y * dragDirections["y"] + z * dragDirections["z"];
                                    draggedObjects[i].transform.rotation = GameObject_DragFrom.transform.rotation;
                                    draggedObjects[i].name= GameObject_DragFrom.name+" ["+i+"]";
                                    draggedObjects[i].SetActive(true);
                                    EffectManager.UpdateGhostVisuals(draggedObjects[i]);
                                    i++;
                                    if (i >= MaxDraggedObjects)
                                        break;
                                }
                            }
                            if (i >= MaxDraggedObjects)
                                break;
                        }
                        if (i >= MaxDraggedObjects)
                            break;
                    }

                    // Disable all pooled Objects that are not in there any more
                    for (int j = i; j < MaxDraggedObjects; j++)
                    {
                        draggedObjects[j].SetActive(false);
                    }
                }
            }
        }

        public void DestroyActiveBlueprint()
        {
            if (ActiveBlueprint != null)
                Destroy(ActiveBlueprint);
        }

        //TO DO : refactor that with love & care
        public BeyondGroup CreateNewBeyondGroup(GameObject go, string name = null)
        {
            // Auto give name
            if (name == null)
                name = String.Format("Group {0:0000}", GameManager.instance.Place.BeyondGroups.Count);

            BeyondComponent bc = go.GetComponent<BeyondComponent>();
            if (bc != null)
            {
                // bc.transform.position - bc.template.pivotOffset : THIS IS ESSENTIAL - This allows us to properly set the pivot of the group 
                BeyondGroup group = new BeyondGroup(name, bc.transform.position - bc.Template.CellCentre, bc.transform.rotation);
                group.AddObject(go);
                GameManager.instance.Place.BeyondGroups.Add(group);
                return group;
            }
            else
            {
                Debug.LogError("CreateNewBeyondGroup attempted to create an emtpy group");
            }
            return null;
        }

    }
}
