using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapMagic;

namespace Beyond
{

    public class MapMagicManager : MonoBehaviour
    {
        [SerializeField] private MapMagic.Core.MapMagicObject mm;
        [SerializeField] private GameObject FPSController;

        public static MapMagicManager instance ;

        private bool initialised;

        private void Awake()
        {
            initialised = false;
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //mm.graph.random.Seed = (int)(System.DateTime.Now.Ticks % 1000000);
            //mm.StartGenerate();
        }

        void Update()
        {
            if (!initialised)
                Initialise();
        }

        void Initialise()
        {
            if (!initialised)
            {
                if (!mm.IsGenerating())
                {
                    FPSController.SetActive(false);
                    Vector3 characterPosition = FPSController.transform.position;
                    Vector3 castPosition = characterPosition;
                    castPosition.y = 1000f;
                    RaycastHit hit;
                    int layerMask = 1 << 8; // Terrain layer
                    Physics.Raycast(castPosition, Vector3.down, out hit, Mathf.Infinity, layerMask);
                    characterPosition.y = hit.point.y + 2f;
                    FPSController.transform.position = characterPosition;
                    FPSController.SetActive(true);
                    initialised = true;
                }
            }
        }

        public float GetLoadProgress()
        {
            return mm.GetProgress();
        }

        public bool MapIsGenerating()
        {
            return mm.IsGenerating();
        }
    }
}
