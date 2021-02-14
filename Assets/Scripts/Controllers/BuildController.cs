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

        [SerializeField] private GameObject FPSCharacter;
        [SerializeField] public GameObject ActiveBlueprint { get; protected set; }
        [SerializeField] public bool CanPlace;

        void Start()
        {
            instance = this;
        }

        void Update()
        {
            // If we have an active blueprint, update its visuals (keep it horizontal, show outline, etc)
            if (ActiveBlueprint != null)
            {
                ActiveBlueprint.transform.localRotation = Quaternion.Inverse(FPSCharacter.transform.localRotation);
                CanPlace = Constraint.CheckConstraint(ActiveBlueprint);
                ActiveBlueprint.GetComponent<Outline>().OutlineColor = (CanPlace ? Color.green : Color.red);
            }
        }

        public void CreateBlueprint(GameAction blueprint, List<GameAction> materials)
        {
            // Instantiate a prefab of that blueprint
            // Show it floating 5 units away from where the FPS is looking
            Template t = GameManager.instance.GetTemplate(blueprint.Name);

            if (t !=null)
            {
                DestroyActiveBlueprint();
                ActiveBlueprint = Instantiate(t.Prefab, FPSCharacter.transform, false);
                BeyondComponent bc = ActiveBlueprint.AddComponent<BeyondComponent>();
                bc.SetTemplate(t);
                ActiveBlueprint.transform.Translate(new Vector3(0, 0, blueprintDistanceFromCamera));
            }
            else
            {
                Debug.LogError("Attempting to create null template");
            }
        }

        public void TryPlacingBlueprint()
        {
            if (ActiveBlueprint!=null)
            {
                if (Constraint.CheckConstraint(ActiveBlueprint))
                {
                    GameObject PlacedObject = Instantiate(ActiveBlueprint , ActiveBlueprint.transform.position , ActiveBlueprint.transform.rotation);
                    Destroy(PlacedObject.GetComponent<Outline>());
                }
                DestroyActiveBlueprint();
            }
        }

        public void DestroyActiveBlueprint()
        {
            if (ActiveBlueprint != null)
                Destroy(ActiveBlueprint);
        }
    }
}
